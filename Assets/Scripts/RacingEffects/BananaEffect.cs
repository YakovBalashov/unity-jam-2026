using RacingGame.Audio;
using RacingGame.Core;
using UnityEngine;

namespace RacingGame.RacingEffects
{
    [CreateAssetMenu(fileName = "BananaEffect", menuName = "Racing/Effects/BananaEffect")]
    public class BananaEffect : RacingEffect
    {
        [SerializeField] private GameObject bananaPrefab;
        [SerializeField] private float animationDuration = 2f;
        [SerializeField] private float stunDuration = 2f;
        [SerializeField] private int turns = 10;
        [SerializeField] private float hopHeight = 0.5f;
        [SerializeField] private float bananaSpawnOffset = 1f;
        [SerializeField] private SoundEffect slipSound;
        
        private const float RayStartHeight = 2f;
        private const float RayDistance = 5f;
        

        protected override void ApplyPositive(CarMovement car)
        {
            var basePosition = car.transform.position - car.transform.forward * bananaSpawnOffset;
            var rayStart = basePosition + Vector3.up * RayStartHeight;

            Instantiate(bananaPrefab,
                Physics.Raycast(rayStart, Vector3.down, out var hit, RayDistance) ? hit.point : basePosition,
                Quaternion.identity);
        }

        protected override void ApplyNegative(CarMovement car)
        {
            car.ApplySpinOut(animationDuration, stunDuration, turns, hopHeight);
            slipSound.Play(car.transform.position);
        }
    }
}