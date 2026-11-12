using UnityEngine;

namespace RacingGame.Core
{
    public class CarMovement : MonoBehaviour
    {
        [SerializeField] private Rigidbody sphereRigidBody;

        [Header("Speed")] [SerializeField] private float maxSpeed;
        [SerializeField] private float maxReverseSpeed;
        [SerializeField] private float acceleration;
        [SerializeField] private float deceleration;
        [SerializeField] private float brakeDeceleration;
        [SerializeField] private float rollingResistance;

        [Header("Ground")] [SerializeField] private float grip;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float groundCheckDistance;
        [SerializeField] private float tiltSpeed = 10f;
        [SerializeField] private float extraJumpGravity;

        [Header("Steering")] [SerializeField] private float turnStrength;
        [SerializeField] private float maxWheelTurn;
        [SerializeField] private float minSpeedForFullSteer;

        [SerializeField] private AnimationCurve steerBySpeedCurve =
            new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0.3f));

        [Header("Visual")] [SerializeField] private Transform model;
        [SerializeField] private Transform leftFrontWheel;
        [SerializeField] private Transform rightFrontWheel;

        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        private float _speedInput;
        private float _steerInput;

        private float _currentTurnRate;

        private float _speedMultiplier = 1f;
        private float _speedModifierTimeLeft;
        private float _speedModifierTransitionAcceleration;

        private float _stunEndTime;
        private bool IsStunned => Time.time < _stunEndTime;

        private float _spinStartTime;
        private float _spinDuration;
        private int _spinTurns;

        private float _hopStartTime;
        private float _hopDuration;
        private float _hopHeight;

        private Renderer[] _modelRenderers;
        private MaterialPropertyBlock _propertyBlock;
        private bool _isTinted;

        private Vector3 _modelStartLocalPosition;
        private Quaternion _modelStartLocalRotation;

        private Vector3 _groundNormal = Vector3.up;
        private Quaternion _currentTilt = Quaternion.identity;

        public void SetInput(float speedInput, float steerInput)
        {
            _speedInput = speedInput;
            _steerInput = steerInput;
        }

        public void ApplySpeedModifier(float multiplier, float duration, float transitionAcceleration)
        {
            _speedMultiplier = multiplier;
            _speedModifierTimeLeft = duration;
            _speedModifierTransitionAcceleration = transitionAcceleration;
        }

        public void ApplySpinOut(float animationDuration, float stunDuration, int turns, float hopHeight)
        {
            StartSpin(turns, animationDuration);
            StartHop(hopHeight, animationDuration);
            Stun(animationDuration + stunDuration);
        }

        public void ApplyExplosion(float stunDuration, Color tintColor,
            float hopHeight, float hopDuration, ParticleSystem particlesPrefab = null)
        {
            Stun(stunDuration);
            ApplyTint(tintColor);
            StartHop(hopHeight, hopDuration);

            if (particlesPrefab)
                Instantiate(particlesPrefab, transform.position, Quaternion.identity);
        }

        private void Start()
        {
            sphereRigidBody.transform.parent = null;

            _modelStartLocalPosition = model.localPosition;
            _modelStartLocalRotation = model.localRotation;

            _modelRenderers = model.GetComponentsInChildren<Renderer>();
            _propertyBlock = new MaterialPropertyBlock();
        }

        private void Update()
        {
            if (!IsStunned)
                transform.Rotate(0f, _currentTurnRate * Time.deltaTime, 0f, Space.World);
        }

        private void LateUpdate()
        {
            transform.position = sphereRigidBody.transform.position;
            UpdateModelVisual();
        }

        private void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;
            UpdateSpeedModifierTimer(dt);

            if (IsStunned)
            {
                StopCar();
                return;
            }

            float forwardSpeed = Vector3.Dot(sphereRigidBody.linearVelocity, transform.forward);

            if (IsGround())
            {
                forwardSpeed = CalculateForwardSpeed(forwardSpeed, dt);
                ApplyGroundVelocity(forwardSpeed, dt);
                _currentTurnRate = CalculateTurnRate(forwardSpeed);
            }
            else
            {
                _currentTurnRate = 0f;
                sphereRigidBody.AddForce(Vector3.down * extraJumpGravity, ForceMode.Acceleration);
            }

            UpdateFrontWheels(forwardSpeed);
        }

        private void Stun(float duration)
        {
            _stunEndTime = Mathf.Max(_stunEndTime, Time.time + duration);
            _currentTurnRate = 0f;
            ClearSpeedModifier();
            StopCar();
        }

        private void StopCar()
        {
            sphereRigidBody.linearVelocity = Vector3.up * sphereRigidBody.linearVelocity.y;
        }

        private void ClearSpeedModifier()
        {
            _speedMultiplier = 1f;
            _speedModifierTimeLeft = 0f;
        }

        private float CalculateForwardSpeed(float forwardSpeed, float dt)
        {
            bool modifierActive = _speedModifierTimeLeft > 0f;
            bool boosting = modifierActive && _speedMultiplier > 1f;
            float input = boosting ? 1f : _speedInput;
            float target, rate;

            switch (input)
            {
                case > 0f:
                    target = maxSpeed * input * _speedMultiplier;
                    rate = forwardSpeed < 0f ? brakeDeceleration : acceleration;
                    break;
                case < 0f:
                    target = maxReverseSpeed * input * _speedMultiplier;
                    rate = forwardSpeed > 0f ? brakeDeceleration : deceleration;
                    break;
                default:
                    target = 0f;
                    rate = rollingResistance;
                    break;
            }

            if (boosting)
            {
                rate = _speedModifierTransitionAcceleration;
            }
            else if (modifierActive)
            {
                float speedLimit = (forwardSpeed >= 0f ? maxSpeed : maxReverseSpeed) * _speedMultiplier;
                if (!(Mathf.Abs(forwardSpeed) > speedLimit)) return Mathf.MoveTowards(forwardSpeed, target, rate * dt);
                target = Mathf.Sign(forwardSpeed) * speedLimit;
                rate = Mathf.Max(rate, _speedModifierTransitionAcceleration);
            }

            return Mathf.MoveTowards(forwardSpeed, target, rate * dt);
        }

        private void ApplyTint(Color color)
        {
            _propertyBlock.SetColor(BaseColorId, color);
            foreach (var modelRenderer in _modelRenderers)
                modelRenderer.SetPropertyBlock(_propertyBlock);

            _isTinted = true;
        }

        private void UpdateTint()
        {
            if (!_isTinted || IsStunned) return;

            foreach (var modelRenderer in _modelRenderers)
                modelRenderer.SetPropertyBlock(null);

            _isTinted = false;
        }

        private void StartSpin(int turns, float duration)
        {
            _spinStartTime = Time.time;
            _spinDuration = duration;
            _spinTurns = turns;
        }

        private void StartHop(float height, float duration)
        {
            _hopStartTime = Time.time;
            _hopDuration = duration;
            _hopHeight = height;
        }

        private void UpdateModelVisual()
        {
            UpdateTilt();
            UpdateTint();
            float angle = 0f;
            float height = 0f;

            float spinProgress = GetProgress(_spinStartTime, _spinDuration);
            if (spinProgress < 1f)
                angle = 360f * _spinTurns * spinProgress;

            float hopProgress = GetProgress(_hopStartTime, _hopDuration);
            if (hopProgress < 1f)
                height = Mathf.Sin(hopProgress * Mathf.PI) * _hopHeight;

            model.localRotation = _currentTilt * _modelStartLocalRotation * Quaternion.Euler(0f, angle, 0f);
            model.localPosition = _modelStartLocalPosition + Vector3.up * height;
        }

        private static float GetProgress(float startTime, float duration)
        {
            return duration > 0f ? (Time.time - startTime) / duration : 1f;
        }

        private static void SetWheelAngle(Transform wheel, float angle)
        {
            Vector3 e = wheel.localEulerAngles;
            wheel.localRotation = Quaternion.Euler(e.x, angle, e.z);
        }

        private void UpdateSpeedModifierTimer(float dt)
        {
            if (_speedModifierTimeLeft <= 0f) return;

            _speedModifierTimeLeft -= dt;
            if (_speedModifierTimeLeft <= 0f) _speedMultiplier = 1f;
        }

        private bool IsGround()
        {
            if (Physics.Raycast(sphereRigidBody.position, Vector3.down, out RaycastHit hit, groundCheckDistance,
                    groundMask))
            {
                _groundNormal = hit.normal;
                return true;
            }

            _groundNormal = Vector3.up;
            return false;
        }

        private void ApplyGroundVelocity(float forwardSpeed, float dt)
        {
            Vector3 velocity = sphereRigidBody.linearVelocity;

            Vector3 sidewaysVelocity = Vector3.Dot(velocity, transform.right) * transform.right;
            sidewaysVelocity = Vector3.Lerp(sidewaysVelocity, Vector3.zero, grip * dt);

            sphereRigidBody.linearVelocity =
                transform.forward * forwardSpeed + sidewaysVelocity + Vector3.up * velocity.y;
        }

        private float CalculateWheelAngle(float forwardSpeed)
        {
            float speed = Mathf.Abs(forwardSpeed);
            return _steerInput * maxWheelTurn * steerBySpeedCurve.Evaluate(speed / maxSpeed);
        }

        private float CalculateTurnRate(float forwardSpeed)
        {
            float speed = Mathf.Abs(forwardSpeed);
            float moveFactor = Mathf.Clamp01(speed / minSpeedForFullSteer);
            float wheelAngle = CalculateWheelAngle(forwardSpeed);

            return (wheelAngle / maxWheelTurn) * turnStrength * moveFactor * Mathf.Sign(forwardSpeed);
        }

        private void UpdateFrontWheels(float forwardSpeed)
        {
            float wheelAngle = CalculateWheelAngle(forwardSpeed);
            SetWheelAngle(leftFrontWheel, wheelAngle);
            SetWheelAngle(rightFrontWheel, wheelAngle);
        }

        private void UpdateTilt()
        {
            Vector3 localNormal = transform.InverseTransformDirection(_groundNormal);
            Quaternion targetTilt = Quaternion.FromToRotation(Vector3.up, localNormal);

            float t = 1f - Mathf.Exp(-tiltSpeed * Time.deltaTime);
            _currentTilt = Quaternion.Slerp(_currentTilt, targetTilt, t);
        }
    }
}