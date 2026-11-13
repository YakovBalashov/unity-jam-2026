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
        [SerializeField] private Material burntMaterial;
        [SerializeField] private float rocketSpawnHeight = 0.5f;

        protected override void ApplyPositive(CarMovement car)
        {
            var flatForward = Vector3.ProjectOnPlane(car.transform.forward, Vector3.up).normalized;
            var position = car.transform.position + flatForward * rocketSpawnOffset + Vector3.up * rocketSpawnHeight;

            Instantiate(rocketPrefab, position, Quaternion.LookRotation(flatForward, Vector3.up));
        }

        protected override void ApplyNegative(CarMovement car)
        {
            car.ApplyExplosion(negativeStunDuration, burntMaterial, hopHeight, negativeHopDuration);
        }
    }
}
