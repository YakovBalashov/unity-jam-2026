using RacingGame.Core;
using UnityEngine;

namespace RacingGame.RacingEffects
{
    [CreateAssetMenu(fileName = "SpeedEffect", menuName = "Racing/Effects/SpeedEffect")]
    public class SpeedEffect : RacingEffect
    {
        [SerializeField] private float accelerationMultiplier = 1.5f;
        [SerializeField] private float decelerationMultiplier = 0.5f;
        [SerializeField] private float accelerationDuration = 5f;
        [SerializeField] private float decelerationDuration = 3f;
        
        private const float InstantTransitionAcceleration = 10000f;
        
        protected override void ApplyPositive(CarMovement car)
        {
            car.ApplySpeedModifier(accelerationMultiplier, accelerationDuration, InstantTransitionAcceleration);
        }

        protected override void ApplyNegative(CarMovement car)
        {
            car.ApplySpeedModifier(decelerationMultiplier, decelerationDuration, InstantTransitionAcceleration);
        }
    }
}
