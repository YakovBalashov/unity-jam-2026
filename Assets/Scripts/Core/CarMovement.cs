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

        [Header("Steering")] [SerializeField] private float turnStrength;
        [SerializeField] private float maxWheelTurn;
        [SerializeField] private float minSpeedForFullSteer;

        [SerializeField] private AnimationCurve steerBySpeedCurve =
            new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0.3f));

        [Header("Wheels")] [SerializeField] private Transform leftFrontWheel;
        [SerializeField] private Transform rightFrontWheel;

        private float _speedInput;
        private float _steerInput;


        public void SetInput(float speedInput, float steerInput)
        {
            _speedInput = speedInput;
            _steerInput = steerInput;
        }

        void Start()
        {
            sphereRigidBody.transform.parent = null;
        }

        void LateUpdate()
        {
            transform.position = sphereRigidBody.transform.position;
        }

        void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;
            bool grounded = Physics.Raycast(sphereRigidBody.position, Vector3.down, groundCheckDistance, groundMask);

            Vector3 velocity = sphereRigidBody.linearVelocity;
            Vector3 transformForward = transform.forward;
            Vector3 transformRight = transform.right;

            float forwardSpeed = Vector3.Dot(velocity, transformForward);

            if (grounded)
            {
                forwardSpeed = CalculateForwardSpeed(forwardSpeed, dt);

                Vector3 lateral = Vector3.Dot(velocity, transformRight) * transformRight;
                lateral = Vector3.Lerp(lateral, Vector3.zero, grip * dt);

                sphereRigidBody.linearVelocity = transformForward * forwardSpeed + lateral + Vector3.up * velocity.y;
            }

            float speed = Mathf.Abs(forwardSpeed);
            float wheelAngle = _steerInput * maxWheelTurn * steerBySpeedCurve.Evaluate(speed / maxSpeed);

            if (grounded)
            {
                float moveFactor = Mathf.Clamp01(speed / minSpeedForFullSteer);
                float turn = (wheelAngle / maxWheelTurn) * turnStrength * moveFactor * Mathf.Sign(forwardSpeed);
                transform.Rotate(0f, turn * dt, 0f, Space.World);
            }

            SetWheelAngle(leftFrontWheel, wheelAngle);
            SetWheelAngle(rightFrontWheel, wheelAngle);
        }

        private float CalculateForwardSpeed(float forwardSpeed, float dt)
        {
            float target, rate;

            switch (_speedInput)
            {
                case > 0f:
                    target = maxSpeed * _speedInput;
                    rate = forwardSpeed < 0f ? brakeDeceleration : acceleration;
                    break;
                case < 0f:
                    target = maxReverseSpeed * _speedInput;
                    rate = forwardSpeed > 0f ? brakeDeceleration : deceleration;
                    break;
                default:
                    target = 0f;
                    rate = rollingResistance;
                    break;
            }

            return Mathf.MoveTowards(forwardSpeed, target, rate * dt);
        }


        private static void SetWheelAngle(Transform wheel, float angle)
        {
            Vector3 e = wheel.localEulerAngles;
            wheel.localRotation = Quaternion.Euler(e.x, angle, e.z);
        }
    }
}