using RacingGame.Core;
using UnityEngine;

namespace RacingGame.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class EngineSound : MonoBehaviour
    {
        [SerializeField] private CarMovement car;

        [Header("Pitch")]
        [SerializeField] private float idlePitch = 0.7f;        
        [SerializeField] private float maxPitch = 1.8f;   
        [SerializeField] private float throttlePitchBoost = 0.15f; 

        [Header("Volume")]
        [SerializeField] private float idleVolume = 0.35f;   
        [SerializeField] private float throttleVolume = 0.9f;  
        [SerializeField] private float coastVolume = 0.55f;  

        [Header("Smoothing")]
        [SerializeField] private float pitchSmoothing = 6f;
        [SerializeField] private float volumeSmoothing = 8f;

        private AudioSource _source;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
            _source.loop = true;
            _source.playOnAwake = false;
        }

        private void Start()
        {
            _source.pitch = idlePitch;
            _source.volume = idleVolume;
            _source.Play();
        }

        private void Update()
        {
            float speedRatio = Mathf.Clamp(car.CurrentSpeed / car.MaxSpeed, 0f, 1.3f);
            float throttle = Mathf.Abs(car.ThrottleInput);

            float targetPitch = Mathf.LerpUnclamped(idlePitch, maxPitch, speedRatio) + throttle * throttlePitchBoost;

            float loadVolume = Mathf.Lerp(coastVolume, throttleVolume, throttle);
            float targetVolume = Mathf.Lerp(idleVolume, loadVolume, Mathf.Max(throttle, speedRatio));

            _source.pitch = Mathf.Lerp(_source.pitch, targetPitch, 1f - Mathf.Exp(-pitchSmoothing * Time.deltaTime));
            _source.volume = Mathf.Lerp(_source.volume, targetVolume, 1f - Mathf.Exp(-volumeSmoothing * Time.deltaTime));
        }
    }
}