using RacingGame.RacingEffects;
using TMPro;
using UnityEngine;

namespace RacingGame.UI
{
    public class PercentageText : MonoBehaviour
    {
        [SerializeField] private EffectManager effectManager;

        private TextMeshProUGUI _textMeshPro;

        private void Awake()
        {
            _textMeshPro = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            var percentage = effectManager.GetCurrentPercentage();
            _textMeshPro.text = $"{percentage}%";
        }
    }
}
