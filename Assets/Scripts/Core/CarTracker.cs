using UnityEngine;

namespace RacingGame.Core
{
    public class CarTracker : MonoBehaviour, System.IComparable<CarTracker>
    {
        public int CheckpointsPassed { get; private set; }
        public float DistanceToNext { get; private set; }
        public int CurrentPosition { get; set; }
        public int LapsCompleted => _track ? CheckpointsPassed / _track.Waypoints.Count : 0;
        [SerializeField] private float passRadius = 10f;

        private RaceTrack _track;
        private int _nextIndex;

        public void Initialize(RaceTrack track)
        {
            _track = track;
            _nextIndex = 1;
            CheckpointsPassed = 0;
        }

        private void Update()
        {
            var wps = _track.Waypoints;
            var count = wps.Count;
            if (count < 2) return;

            for (var i = 0; i < 3; i++)
            {
                var prev = wps[(_nextIndex - 1 + count) % count].position;
                var next = wps[_nextIndex].position;
                var dir = (next - prev).normalized;

                var crossedPlane = Vector3.Dot(transform.position - next, dir) >= 0f;
                var withinRadius = (transform.position - next).sqrMagnitude < passRadius * passRadius;

                if (!crossedPlane && !withinRadius) break;

                _nextIndex = (_nextIndex + 1) % count;
                CheckpointsPassed++;
            }

            DistanceToNext = Vector3.Distance(transform.position, wps[_nextIndex].position);
        }

        public int CompareTo(CarTracker other)
        {
            return CheckpointsPassed != other.CheckpointsPassed
                ? other.CheckpointsPassed.CompareTo(CheckpointsPassed)
                : DistanceToNext.CompareTo(other.DistanceToNext);
        }
    }
}