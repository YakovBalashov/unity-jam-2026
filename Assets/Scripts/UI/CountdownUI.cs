using System.Collections;
using TMPro;
using UnityEngine;

namespace RacingGame.UI
{
    public class CountdownUI : MonoBehaviour
    {
        private TextMeshProUGUI _textMeshPro;
        
        private void Awake()
        {
            _textMeshPro = GetComponent<TextMeshProUGUI>();
        }

        public void Show(string value)
        {
            _textMeshPro.gameObject.SetActive(true);
            _textMeshPro.text = value;
            StopAllCoroutines();
            StartCoroutine(Pop());
        }

        public void Hide() => _textMeshPro.gameObject.SetActive(false);

        private IEnumerator Pop()
        {
            const float duration = 0.4f;
            for (var t = 0f; t < duration; t += Time.deltaTime)
            {
                var k = t / duration;
                _textMeshPro.transform.localScale = Vector3.one * Mathf.Lerp(1.6f, 1f, k * k);
                yield return null;
            }
            _textMeshPro.transform.localScale = Vector3.one;
        }
    }
}
