using RacingGame.Core;
using RacingGame.Extensions;
using RacingGame.RacingEffects;
using UnityEngine;
using Random = System.Random;

namespace RacingGame.AI
{
    [RequireComponent(typeof(CarMovement), typeof(Rigidbody), typeof(EffectManager))]
    public class CarAIController : MonoBehaviour
    {
        private const float MaxInput = 1f;

        [Header("Pathfinding")]
        [SerializeField] private float waypointTolerance = 8f;
        
        [Header("Steering")]
        [SerializeField] private float steeringSensitivity = 5f;
        
        [Header("Speed Control")]
        [SerializeField] private float fullAheadAngle = 0f;
        [SerializeField] private float fullStopAngle = 90f;
        
        [Header("Recovery")]
        [SerializeField] private float stuckSpeedThreshold = 1f;
        [SerializeField] private float stuckTime = 1.5f;
        [SerializeField] private float reverseTime = 1.2f;
        [SerializeField] private float creepThrottle = 0.3f;

        private float _stuckTimer;
        private float _reverseTimer;

        private RaceTrack _trackPath;
        private CarMovement _carMovement;
        private EffectManager _effectManager;
        private Rigidbody _rigidbody;
        private int _currentWaypointIndex = 0;
        private int _targetPercentage;
        private int _currentTargetPercentage;
        private int _percentageOffset;
        
        private Random _random = new();

        private void Awake()
        {
            _carMovement = GetComponent<CarMovement>();
            _rigidbody = GetComponent<Rigidbody>();
            _effectManager = GetComponent<EffectManager>();
        }

        public void Initialize(RaceTrack track, float sensitivity, int percentage, int percentageOffset)
        {
            _trackPath = track;
            steeringSensitivity = sensitivity;
            _targetPercentage = percentage;
            _percentageOffset = percentageOffset;
            _currentTargetPercentage = _random.RandomizeValue(_targetPercentage, _percentageOffset);
        }

        private void Update()
        {
            if (!_trackPath || _trackPath.AIWaypoints.Count == 0) return;

            UpdateTargetWaypoint();
            DriveTowardsWaypoint();
            ApplyEffect();
        }

        private void ApplyEffect()
        {
            if (_effectManager.GetCurrentPercentage() < _currentTargetPercentage) return;
            _effectManager.ApplyCurrentEffect();
            _currentTargetPercentage = _random.RandomizeValue(_targetPercentage, _percentageOffset);
        }

        private void UpdateTargetWaypoint()
        {
            var wps = _trackPath.AIWaypoints;
            var count = wps.Count;
            var prev = wps[(_currentWaypointIndex - 1 + count) % count].position;
            var next = wps[_currentWaypointIndex].position;

            var reached = Vector3.Distance(transform.position, next) < waypointTolerance;
            var passed = Vector3.Dot(transform.position - next, (next - prev).normalized) > 0f;

            if (reached || passed)
                _currentWaypointIndex = (_currentWaypointIndex + 1) % count;
        }

        private void DriveTowardsWaypoint()
        {
            var targetPosition = _trackPath.AIWaypoints[_currentWaypointIndex].position;
            var directionToTarget = (targetPosition - transform.position).normalized;
            var localDirection = transform.InverseTransformDirection(directionToTarget);

            var steerInput = Mathf.Clamp(localDirection.x * steeringSensitivity, -MaxInput, MaxInput);

            if (localDirection.z < 0f)
                steerInput = localDirection.x >= 0f ? MaxInput : -MaxInput;

            var forwardSpeed = Vector3.Dot(_rigidbody.linearVelocity, transform.forward);

            if (Mathf.Abs(forwardSpeed) < stuckSpeedThreshold) _stuckTimer += Time.deltaTime;
            else _stuckTimer = 0f;

            if (_stuckTimer >= stuckTime)
            {
                _stuckTimer = 0f;
                _reverseTimer = reverseTime;
            }

            if (_reverseTimer > 0f)
            {
                _reverseTimer -= Time.deltaTime;
                _carMovement.SetInput(-MaxInput, -steerInput);
                return;
            }

            var angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

            var minInput = forwardSpeed > stuckSpeedThreshold ? -MaxInput : creepThrottle;
            var speedInput = Mathf.Lerp(MaxInput, minInput, Mathf.InverseLerp(fullAheadAngle, fullStopAngle, angleToTarget));

            _carMovement.SetInput(speedInput, steerInput);
        }
    }
}
