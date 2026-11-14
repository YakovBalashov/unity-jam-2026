using UnityEngine;

namespace RacingGame.UI
{
    public class PulseScale : MonoBehaviour
    {
        [SerializeField] private float amplitude = 0.08f; 
        [SerializeField] private float frequency = 1f;

        private Vector3 _baseScale;

        private void Awake()
        {
            _baseScale = transform.localScale;
        }

        private void Update()
        {
            float scale = 1f + Mathf.Sin(Time.unscaledTime * frequency * Mathf.PI * 2f) * amplitude;
            transform.localScale = _baseScale * scale;
        }
    }
}