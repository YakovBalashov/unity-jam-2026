using RacingGame.Core;
using UnityEngine;

namespace RacingGame.RacingEffects
{
    public class Banana : MonoBehaviour
    {
        [SerializeField] private float animationDuration = 2f;
        [SerializeField] private float stunDuration = 2f;
        [SerializeField] private int turns = 10;
        [SerializeField] private float hopHeight = 0.5f;
        
        private void OnTriggerEnter(Collider other)
        {
            var carMovement = other.GetComponentInParent<CarMovement>();
            if (carMovement == null) return;
            carMovement.ApplySpinOut(animationDuration, stunDuration, turns, hopHeight);
            Destroy(gameObject);
        }
    }
}