using RacingGame.Core;
using UnityEngine;

namespace RacingGame.RacingEffects
{
    public class Rocket : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private float duration;
        [SerializeField] private float hopHeight;

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
            if (carMovement != null) carMovement.ApplyExplosion(duration, Color.white, hopHeight, duration);
            Destroy(gameObject);
        }
    }
}