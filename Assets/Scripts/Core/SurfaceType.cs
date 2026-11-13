using UnityEngine;

namespace RacingGame.Core
{
    [CreateAssetMenu(fileName = "SurfaceType", menuName = "Racing/Surface Type")]
    public class SurfaceType : ScriptableObject
    {
        [SerializeField, Range(0.1f, 1f)] private float speedMultiplier = 0.6f;
        [SerializeField, Range(0f, 2f)] private float gripMultiplier = 1f;

        public float SpeedMultiplier => speedMultiplier;
        public float GripMultiplier => gripMultiplier;
    }
}