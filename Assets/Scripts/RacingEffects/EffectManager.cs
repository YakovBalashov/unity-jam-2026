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

        private float _currentPercentage;
        private RacingEffect _currentEffect;

        private CarMovement _carMovement;

        private void Awake()
        {
            _currentPercentage = 0;
            _currentEffect = availableEffects[Random.Range(0, availableEffects.Count)];

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
            _currentEffect.Apply(Mathf.FloorToInt(_currentPercentage), _carMovement);
            _currentPercentage = 0;
            _currentEffect = availableEffects[Random.Range(0, availableEffects.Count)];
        }

        private void IncreasePercentage(float deltaTime)
        {
            _currentPercentage += percentPerSecond * deltaTime;
            _currentPercentage = Mathf.Clamp(_currentPercentage, 0, RacingEffect.MaxPercentage);
        }
    }
}