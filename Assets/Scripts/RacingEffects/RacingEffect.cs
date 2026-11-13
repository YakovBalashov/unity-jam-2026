using RacingGame.Core;
using UnityEngine;

namespace RacingGame.RacingEffects
{
    public abstract class RacingEffect : ScriptableObject
    {
        public const int MaxPercentage = 100;
        [field: SerializeField] public Sprite Icon { get; private set; }

        public void Apply(int positivePercentage, CarMovement car)
        {
            if (Random.Range(0, MaxPercentage) < positivePercentage)
            {
                ApplyPositive(car);
            }
            else
            {
                ApplyNegative(car);
            }
        }

        protected abstract void ApplyPositive(CarMovement car);
        protected abstract void ApplyNegative(CarMovement car);
    }
}