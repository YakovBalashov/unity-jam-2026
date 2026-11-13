using RacingGame.Core;
using UnityEngine;

namespace RacingGame.RacingEffects
{
    public class Rocket : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private float stunDuration= 1f;
        [SerializeField] private float hopDuration= 0.4f;
        [SerializeField] private float hopHeight = 0.5f;
        [SerializeField] private Material burntMaterial;

        private Vector3 _velocity;

        private void Awake()
        {
            _velocity = speed * transform.forward;
        }

        private void Update()
        {
            transform.Translate(_velocity * Time.deltaTime, Space.World);
        }

        private void OnTriggerEnter(Collider other)
        {
            var carMovement = other.GetComponentInParent<CarMovement>();
            if (carMovement != null) carMovement.ApplyExplosion(stunDuration, burntMaterial, hopHeight, hopDuration);
            Destroy(gameObject);
        }
    }
}