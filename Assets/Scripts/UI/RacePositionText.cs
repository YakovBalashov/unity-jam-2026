using RacingGame.Core;
using UnityEngine;

namespace RacingGame.UI
{
    [RequireComponent(typeof(TMPro.TextMeshProUGUI))]
    public class RacePositionText : MonoBehaviour
    {
        [SerializeField] private CarTracker playerCarTracker;
        
        private TMPro.TextMeshProUGUI _textMeshPro;
        
        private void Awake()
        {
            _textMeshPro = GetComponent<TMPro.TextMeshProUGUI>();
        }
        
        private void Update()
        {
            if (!playerCarTracker) return;
            _textMeshPro.text = GetOrdinalSuffix(playerCarTracker.CurrentPosition);
        }

        private static string GetOrdinalSuffix(int position)
        {
            return (position % 10) switch
            {
                1 => position + "st",
                2 => position + "nd",
                3 => position + "rd",
                _ => position + "th"
            };
        }
    }
}
