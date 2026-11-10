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
        }
    }
}