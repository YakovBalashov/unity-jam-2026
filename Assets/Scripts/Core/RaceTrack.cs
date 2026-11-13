using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RacingGame.Core
{
    public class RaceTrack : MonoBehaviour
    {
        [SerializeField] private int lapCount;

        [field: SerializeField] private Transform trackWaypointRoot;
        [field: SerializeField] public List<Transform> Waypoints { get; private set; } = new();

        [field: SerializeField] private Transform aiWaypointRoot;
        [field: SerializeField] public List<Transform> AIWaypoints { get; private set; } = new();

        [field: SerializeField] public Transform startingPositionRoot;
        [field: SerializeField] public List<Transform> StartingPositions { get; private set; } = new();


        [Header("Gizmo Settings")] [SerializeField]
        private Color trackGizmoColor = Color.orange;

        [SerializeField] private float trackWaypointRadius = 0.5f;

        [SerializeField] private float aiWaypointRadius = 0.5f;
        [SerializeField] private Color aiWaypointColor = Color.magenta;

        [SerializeField] private Color startingPositionColor = Color.green;
        [SerializeField] private float startingPositionRadius = 0.75f;

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

#if UNITY_EDITOR
        private void SyncWaypointsWithHierarchy()
        {
            if (Application.isPlaying) return;

            var needsUpdate = IsDesynced(trackWaypointRoot, Waypoints) ||
                              IsDesynced(aiWaypointRoot, AIWaypoints) ||
                              IsDesynced(startingPositionRoot, StartingPositions);

            if (!needsUpdate) return;

            UpdateMarkdown();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif

        private static bool IsDesynced(Transform root, List<Transform> list)
        {
            if (root == null) return false;

            if (list.Count != root.childCount) return true;

            for (var i = 0; i < root.childCount; i++)
            {
                if (list[i] != root.GetChild(i)) return true;
            }

            return false;
        }

        [ContextMenu("Get Metrics From Children")]
        private void UpdateMarkdown()
        {
            PopulateListFromRoot(trackWaypointRoot, Waypoints);
            PopulateListFromRoot(aiWaypointRoot, AIWaypoints);
            PopulateListFromRoot(startingPositionRoot, StartingPositions);
        }

        private static void PopulateListFromRoot(Transform root, List<Transform> list)
        {
            if (root == null) return;
    
            list.Clear();
            list.AddRange(root.Cast<Transform>());
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            SyncWaypointsWithHierarchy();
#endif

            DrawWaypointGizmos(Waypoints, trackGizmoColor, trackWaypointRadius);
            DrawWaypointGizmos(AIWaypoints, aiWaypointColor, aiWaypointRadius);
            DrawStartingPositionsGizmos(StartingPositions, startingPositionColor, startingPositionRadius);
        }

        private static void DrawStartingPositionsGizmos(List<Transform> startingPositions, Color color, float radius)
        {
            if (startingPositions == null || startingPositions.Count == 0) return;

            Gizmos.color = color;
            foreach (var position in startingPositions.Where(position => position != null))
            {
                Gizmos.DrawSphere(position.position, radius);
            }
        }

        private static void DrawWaypointGizmos(List<Transform> waypoints, Color color, float radius)
        {
            if (waypoints == null || waypoints.Count < 2) return;

            Gizmos.color = color;
            for (var i = 0; i < waypoints.Count; i++)
            {
                var currentWaypoint = waypoints[i];

                if (currentWaypoint == null) continue;

                var nextWaypoint = waypoints[(i + 1) % waypoints.Count];
                if (nextWaypoint == null) continue;

                Gizmos.DrawSphere(currentWaypoint.position, radius);
                Gizmos.DrawLine(currentWaypoint.position, nextWaypoint.position);
            }
        }
    }
}