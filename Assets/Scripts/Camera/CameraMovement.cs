using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RacingGame.Camera
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private InputActionReference lookAction;
        [SerializeField] private float deadZone = 0.05f;

        private CinemachineOrbitalFollow _orbitalFollow;
        private float _targetYaw;

        private void Awake()
        {
            _orbitalFollow = GetComponent<CinemachineOrbitalFollow>();
            _orbitalFollow.HorizontalAxis.Recentering.Enabled = false;
        }

        private void Update()
        {
            var input = lookAction.action.ReadValue<Vector2>();

            _targetYaw = (input.sqrMagnitude > deadZone * deadZone)
                ? Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg
                : 0f;

            _orbitalFollow.HorizontalAxis.Value = _targetYaw;
        }
    }
}