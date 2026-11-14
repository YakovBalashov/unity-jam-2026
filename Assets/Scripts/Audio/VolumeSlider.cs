using UnityEngine;
using UnityEngine.UI;

namespace RacingGame.Audio
{
    [RequireComponent(typeof(Slider))]
    public class VolumeSlider : MonoBehaviour
    {
        [SerializeField] private VolumeChannel channel;

        private Slider _slider;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            _slider.minValue = 0f;
            _slider.maxValue = 1f;
        }

        private void OnEnable()
        {
            _slider.SetValueWithoutNotify(AudioManager.Instance.GetVolume(channel));
            _slider.onValueChanged.AddListener(OnSliderChanged);
        }

        private void OnDisable()
        {
            _slider.onValueChanged.RemoveListener(OnSliderChanged);
        }

        private void OnSliderChanged(float value)
        {
            AudioManager.Instance.SetVolume(channel, value);
        }
    }
}