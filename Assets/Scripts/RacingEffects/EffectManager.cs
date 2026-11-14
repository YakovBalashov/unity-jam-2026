using System;
using System.Collections.Generic;
using RacingGame.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RacingGame.RacingEffects
{
    [RequireComponent(typeof(CarMovement))]
    public class EffectManager : MonoBehaviour
    {
        [SerializeField] private List<RacingEffect> availableEffects;
        [SerializeField] private float percentPerSecond = 4f;

        public Action<RacingEffect> onEffectChanged;
        
        private float _currentPercentage;
        private RacingEffect _currentEffect;

        private CarMovement _carMovement;
        
        public Action<bool> onEffectApplied;

        private void Start()
        {
            _currentPercentage = 0;
            _currentEffect = availableEffects[Random.Range(0, availableEffects.Count)];
            onEffectChanged?.Invoke(_currentEffect);

            _carMovement = GetComponent<CarMovement>();
        }

        private void Update()
        {
            IncreasePercentage(Time.deltaTime);
        }
        
        public int GetCurrentPercentage()
        {
            return Mathf.FloorToInt(_currentPercentage);
        }

        public void ApplyCurrentEffect()
        {
            var result = _currentEffect.Apply(Mathf.FloorToInt(_currentPercentage), _carMovement);
            _currentPercentage = 0;
            _currentEffect = availableEffects[Random.Range(0, availableEffects.Count)];
            onEffectChanged?.Invoke(_currentEffect);
            onEffectApplied?.Invoke(result);
        }

        private void IncreasePercentage(float deltaTime)
        {
            _currentPercentage += percentPerSecond * deltaTime;
            _currentPercentage = Mathf.Clamp(_currentPercentage, 0, RacingEffect.MaxPercentage);
        }
        
        
    }
}