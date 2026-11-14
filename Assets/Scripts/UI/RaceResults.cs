using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace RacingGame.UI
{
    public class RaceResults : MonoBehaviour
    {
        [SerializeField] private string playerName = "YOU";
        [SerializeField] private string highlightColor = "#FD9C00";

        private TextMeshProUGUI _resultsText;
        private readonly StringBuilder _stringBuilder = new();

        private void Awake()
        {
            _resultsText = GetComponent<TextMeshProUGUI>();
            _resultsText.richText = true;
            _resultsText.text = string.Empty;
        }

        public void Show(IReadOnlyList<(int position, string name, float time)> results)
        {
            _stringBuilder.Clear();
            foreach (var (position, carName, time) in results)
            {
                var cleanName = carName.Split('(')[0].Trim();
                var line = $"{position}. {FormatTime(time)} {cleanName}";

                _stringBuilder.AppendLine(cleanName == playerName ? $"<color={highlightColor}>{line}</color>" : line);
            }
            _resultsText.text = _stringBuilder.ToString();
        }

        private static string FormatTime(float seconds)
        {
            var minutes = (int)(seconds / 60f);
            var remainder = seconds - minutes * 60f;
            return $"{minutes}:{remainder:00.00}";
        }
    }
}