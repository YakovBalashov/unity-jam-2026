using RacingGame.Core;
using TMPro;
using UnityEngine;

namespace RacingGame.UI
{
    public class LapText : MonoBehaviour
    {
        [SerializeField] private CarTracker playerCarTracker;
        [SerializeField] private RaceTrack raceTrack;
        
        private TextMeshProUGUI _lapText;
        
        private void Awake()
        {
            _lapText = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            if (!playerCarTracker) return;
            var currentLap = Mathf.Max(raceTrack.LapCount, playerCarTracker.LapsCompleted + 1);
            _lapText.text = $"Lap {currentLap}/{raceTrack.LapCount}";
        }
    }
}
