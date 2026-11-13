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
            CurrentWaypointIndex = 0;
        }
        
        private void Update()
        {
            if (!_trackPath || _trackPath.Waypoints.Count == 0) return;

            UpdateWaypointProgress();
            CalculateContinuousDistance();
        }

        private void UpdateWaypointProgress()
        {
            var count = _trackPath.Waypoints.Count;
            var prev = _trackPath.Waypoints[(CurrentWaypointIndex - 1 + count) % count].position;
            var next = _trackPath.Waypoints[CurrentWaypointIndex].position;

            var segment = next - prev;
            var progress = Vector3.Dot(transform.position - prev, segment.normalized);

            if (progress < segment.magnitude) return;

            CurrentWaypointIndex = (CurrentWaypointIndex + 1) % count;
            if (CurrentWaypointIndex == 1) LapsCompleted++; 
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
