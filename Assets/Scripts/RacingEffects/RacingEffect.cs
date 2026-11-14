using RacingGame.Core;
using UnityEngine;

namespace RacingGame.RacingEffects
{
    public abstract class RacingEffect : ScriptableObject
    {
        public const int MaxPercentage = 100;
        [field: SerializeField] public Sprite Icon { get; private set; }

        public bool Apply(int positivePercentage, CarMovement car)
        {
            if (Random.Range(0, MaxPercentage) < positivePercentage)
            {
                ApplyPositive(car);
                return true;
            }
            ApplyNegative(car);
            return false;
        }
        
        protected static bool IsPlayer(CarMovement car)
        {
            return car.TryGetComponent<PlayerController>(out _);
        }

        protected abstract void ApplyPositive(CarMovement car);
        protected abstract void ApplyNegative(CarMovement car);
    }
}