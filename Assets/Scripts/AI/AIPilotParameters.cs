using UnityEngine;

namespace RacingGame.AI
{
    [CreateAssetMenu(fileName = "AIPilotParameters", menuName = "Racing/AIPilotParameters")]
    public class AIPilotParameters : ScriptableObject
    {
        [field: SerializeField] public float SteeringSensitivity { get; private set; } = 5f;
        [field: SerializeField] public float SteeringSensitivityOffset { get; private set; } = 1f;
        
        [field: SerializeField] public float MaxSpeed { get; private set; } = 25f;
        [field: SerializeField] public float MaxSpeedOffset { get; private set; } = 5f;
        
        [field: SerializeField] public float Acceleration { get; private set; } = 12f;
        [field: SerializeField] public float AccelerationOffset { get; private set; } = 2f;
    }
}
