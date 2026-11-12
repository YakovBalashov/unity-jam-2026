using RacingGame.RacingEffects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RacingGame.Core
{
    [RequireComponent(typeof(CarMovement), typeof(EffectManager))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private InputActionReference speedAction;
        [SerializeField] private InputActionReference steeringAction;
        [SerializeField] private InputActionReference effectAction;
        
        private CarMovement _carMovement;
        private EffectManager _effectManager;

        private void Awake()
        {
            _carMovement = GetComponent<CarMovement>();
            _effectManager = GetComponent<EffectManager>();
        }

        private void Update()
        {
            _carMovement.SetInput(speedAction.action.ReadValue<float>(), steeringAction.action.ReadValue<float>());
        }

        private void UseEffect(InputAction.CallbackContext context)
        {
            _effectManager.ApplyCurrentEffect();
        }
        
        private void OnEnable()
        {
            effectAction.action.performed += UseEffect;
        }
        
        private void OnDisable()
        {
            effectAction.action.performed -= UseEffect;
        }
    }
}