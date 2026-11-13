using UnityEngine;

namespace RacingGame.Core
{
    public class Surface : MonoBehaviour
    {
        [SerializeField] private SurfaceType surfaceType;

        public float SpeedMultiplier => surfaceType ? surfaceType.SpeedMultiplier : 1f;
        public float GripMultiplier => surfaceType ? surfaceType.GripMultiplier : 1f;
    }
}