using System;
using RacingGame.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RacingGame.UI
{
    public enum Difficulty
    {
        Easy,
        Normal,
        Hard
    }

    public class RaceSettingsPanel : MonoBehaviour
    {
        [SerializeField] private Slider lapsSlider;
        [SerializeField] private TMP_Text lapsLabel;
        [SerializeField] private TMP_Dropdown difficultyDropdown;
        [SerializeField] private Button startButton;
        [SerializeField] private int minLaps = 1;
        [SerializeField] private int maxLaps = 5;
        
        public event Action<int, Difficulty> StartPressed; 

        private void Awake()
        {
            lapsSlider.wholeNumbers = true;
            lapsSlider.minValue = minLaps;
            lapsSlider.maxValue = maxLaps;
            lapsSlider.onValueChanged.AddListener(UpdateLapsLabel);

            difficultyDropdown.ClearOptions();
            difficultyDropdown.AddOptions(
                new System.Collections.Generic.List<string>(Enum.GetNames(typeof(Difficulty))));
            
            UpdateLapsLabel(lapsSlider.value);
            Time.timeScale = 0f;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void Hide() => gameObject.SetActive(false);

        private void UpdateLapsLabel(float value) => lapsLabel.text = $"Laps: {(int)value}";

        public void OnStartClicked()
        {
            var laps = (int)lapsSlider.value;
            var difficulty = (Difficulty)difficultyDropdown.value;
            Time.timeScale = 1f;
            StartPressed?.Invoke(laps, difficulty);
        }
    }
}