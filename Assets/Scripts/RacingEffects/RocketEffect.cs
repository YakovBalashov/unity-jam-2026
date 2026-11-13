using RacingGame.Core;
using UnityEngine;

namespace RacingGame.RacingEffects
{
    [CreateAssetMenu(fileName = "RocketEffect", menuName = "Racing/Effects/RocketEffect")]
    public class RocketEffect : RacingEffect
    {
        [SerializeField] private GameObject rocketPrefab;
        [SerializeField] private float positiveDuration = 2f;
        [SerializeField] private float negativeStunDuration = 1f;
        [SerializeField] private float negativeHopDuration = 0.4f;
        [SerializeField] private float hopHeight = 0.5f;
        [SerializeField] private float rocketSpawnOffset = 2f;
        
        protected override void ApplyPositive(CarMovement car)
        {
            Instantiate(rocketPrefab, car.transform.position + car.transform.forward * rocketSpawnOffset, car.transform.rotation);
        }

        protected override void ApplyNegative(CarMovement car)
        {
            car.ApplyExplosion(negativeStunDuration, Color.black, hopHeight, negativeHopDuration);
        }
    }
}
