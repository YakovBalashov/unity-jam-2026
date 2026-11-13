using System.Collections.Generic;
using UnityEngine;

namespace RacingGame.Core
{
    public class RaceTrack : MonoBehaviour
    {
        [SerializeField] private int lapCount;
        [field: SerializeField] public List<Transform> Waypoints { get; private set; } = new();
        
        [Header("Gizmo Settings")]
        [SerializeField] private Color gizmoColor = Color.yellow;
        [SerializeField] private float waypointRadius = 0.5f;

        public List<float> CumulativeDistances { get; private set; }
        public float TrackLength { get; private set; }

        private void Awake()
        {
            if (Waypoints == null || Waypoints.Count < 2) return;
            
            CumulativeDistances = new List<float>(Waypoints.Count);
            var distance = 0f;

            for (var i = 0; i < Waypoints.Count; i++)
            {
                CumulativeDistances.Add(distance);
                
                var nextIndex = (i + 1) % Waypoints.Count;
                distance += Vector3.Distance(Waypoints[i].position, Waypoints[nextIndex].position);
            }
            
            TrackLength = distance;
        }

        [ContextMenu("Get Waypoints From Children")]
        private void GetWaypoints()
        {
            Waypoints.Clear();
            foreach (Transform child in transform)
            {
                Waypoints.Add(child);
            }
        }

        private void OnDrawGizmos()
        {
            if (Waypoints == null || Waypoints.Count < 2) return;

            Gizmos.color = gizmoColor;
            for (var i = 0; i < Waypoints.Count; i++)
            {
                var currentWaypoint = Waypoints[i];
                var nextWaypoint = Waypoints[(i + 1) % Waypoints.Count];

                Gizmos.DrawSphere(currentWaypoint.position, waypointRadius);
                Gizmos.DrawLine(currentWaypoint.position, nextWaypoint.position);
            }
        }
    }
}