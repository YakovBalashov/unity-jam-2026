using UnityEngine;

namespace RacingGame.Core
{
    public class CarTracker : MonoBehaviour, System.IComparable<CarTracker>
    {
        [SerializeField] private float waypointTolerance = 4f;

        public int LapsCompleted { get; private set; }
        public int CurrentWaypointIndex { get; private set; }
        public float TotalRaceDistance { get; private set; }
        public int CurrentPosition { get; set; }
        
        private RaceTrack _trackPath;

        public void Initialize(RaceTrack trackPath)
        {
            _trackPath = trackPath;
            CurrentWaypointIndex = 1;
        }
        
        private void Update()
        {
            if (!_trackPath || _trackPath.Waypoints.Count == 0) return;

            UpdateWaypointProgress();
            CalculateContinuousDistance();
            
        }

        private void UpdateWaypointProgress()
        {
            var targetPosition = _trackPath.Waypoints[CurrentWaypointIndex].position;

            if (!(Vector3.Distance(transform.position, targetPosition) < waypointTolerance)) return;
            CurrentWaypointIndex++;

            if (CurrentWaypointIndex < _trackPath.Waypoints.Count) return;
            CurrentWaypointIndex = 0;

            if (CurrentWaypointIndex != 1) return;
            LapsCompleted++;
        }

        private void CalculateContinuousDistance()
        {
            var previousIndex = CurrentWaypointIndex == 0 ? _trackPath.Waypoints.Count - 1 : CurrentWaypointIndex - 1;
            
            var previousPosition = _trackPath.Waypoints[previousIndex].position;
            var nextPosition = _trackPath.Waypoints[CurrentWaypointIndex].position;

            var segmentDirection = (nextPosition - previousPosition).normalized;
            
            var carToPrev = transform.position - previousPosition;

            var distanceAlongSegment = Vector3.Dot(carToPrev, segmentDirection);

            var lapDistance = _trackPath.CumulativeDistances[previousIndex] + distanceAlongSegment;

            TotalRaceDistance = LapsCompleted * _trackPath.TrackLength + lapDistance;
        }

        public int CompareTo(CarTracker other)
        {
            return other.TotalRaceDistance.CompareTo(TotalRaceDistance);
        }
    }
}
