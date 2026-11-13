using RacingGame.Core;
using UnityEngine;

namespace RacingGame.AI
{
    [RequireComponent(typeof(CarMovement), typeof(Rigidbody))]
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

        private RaceTrack _trackPath;
        private CarMovement _carMovement;
        private Rigidbody _rigidbody;
        private int _currentWaypointIndex = 1;

        private void Awake()
        {
            _carMovement = GetComponent<CarMovement>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void Initialize(RaceTrack track)
        {
            _trackPath = track;
        }

        private void Update()
        {
            if (!_trackPath || _trackPath.AIWaypoints.Count == 0) return;

            UpdateTargetWaypoint();
            DriveTowardsWaypoint();
        }

        private void UpdateTargetWaypoint()
        {
            var targetPosition = _trackPath.AIWaypoints[_currentWaypointIndex].position;
            
            if (Vector3.Distance(transform.position, targetPosition) < waypointTolerance)
            {
                _currentWaypointIndex = (_currentWaypointIndex + 1) % _trackPath.AIWaypoints.Count;
            }
        }

        private void DriveTowardsWaypoint()
        {
            var targetPosition = _trackPath.AIWaypoints[_currentWaypointIndex].position;
    
            var directionToTarget = (targetPosition - transform.position).normalized;
    
            var localDirection = transform.InverseTransformDirection(directionToTarget);
    
            var steerInput = Mathf.Clamp(localDirection.x * steeringSensitivity, -MaxInput, MaxInput);

            var angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
            
            var forwardSpeed = Vector3.Dot(_rigidbody.linearVelocity, transform.forward);
            
            var minInput = forwardSpeed > 0f ? -MaxInput : 0f;

            var speedInput = Mathf.Lerp(MaxInput, minInput, Mathf.InverseLerp(fullAheadAngle, fullStopAngle, angleToTarget));

            _carMovement.SetInput(speedInput, steerInput);
        }
    }
}
