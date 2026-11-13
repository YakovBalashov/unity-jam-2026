using System.Collections.Generic;
using UnityEngine;

namespace RacingGame.Core
{
    public class RaceTrack : MonoBehaviour
    {
        [SerializeField] private Color gizmoColor = Color.yellow;
        [SerializeField] private float waypointRadius = 0.5f;

        [SerializeField] private List<Transform> waypoints = new();

        [ContextMenu("Get Waypoints From Children")]
        private void GetWaypoints()
        {
            waypoints.Clear();
            foreach (Transform child in transform)
            {
                waypoints.Add(child);
            }
        }

        private void OnDrawGizmos()
        {
            if (waypoints == null || waypoints.Count < 2) return;

            Gizmos.color = gizmoColor;
            for (var i = 0; i < waypoints.Count; i++)
            {
                var currentWaypoint = waypoints[i];
                var nextWaypoint = waypoints[(i + 1) % waypoints.Count];

                Gizmos.DrawSphere(currentWaypoint.position, waypointRadius);
                Gizmos.DrawLine(currentWaypoint.position, nextWaypoint.position);
            }
        }
    }
}