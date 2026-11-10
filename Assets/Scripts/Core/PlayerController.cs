using UnityEngine;
using UnityEngine.InputSystem;

namespace RacingGame.Core
{
    [RequireComponent(typeof(CarMovement))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private InputActionReference speedAction;
        [SerializeField] private InputActionReference steeringAction;
        private CarMovement _carMovement;
        
        
        void Awake()
        {
            _carMovement = GetComponent<CarMovement>();
        }

        void Update()
        {
            _carMovement.SetInput(speedAction.action.ReadValue<float>(), steeringAction.action.ReadValue<float>());
            
            // test
            var keyboard = UnityEngine.InputSystem.Keyboard.current;
            if (keyboard.eKey.wasPressedThisFrame) _carMovement.ApplySpeedModifier(1.5f, 2f, 200f);
            if (keyboard.qKey.wasPressedThisFrame) _carMovement.ApplySpeedModifier(0.4f, 2f, 30f);
            if (keyboard.rKey.wasPressedThisFrame) _carMovement.ApplySpinOut(0.5f, 0.15f, 1, 0.5f);
            if (keyboard.tKey.wasPressedThisFrame) _carMovement.ApplyExplosion(1f, Color.black, 0.8f, 0.4f);
        }
    }
}