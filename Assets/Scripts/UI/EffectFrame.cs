using System.Collections.Generic;
using RacingGame.RacingEffects;
using UnityEngine;
using UnityEngine.UI;

namespace RacingGame.UI
{
    public class EffectFrame : MonoBehaviour
    {
        [SerializeField] private EffectManager playerEffectManager;
        [SerializeField] private GameObject frame;
        [SerializeField] private Color positiveColor = Color.green;
        [SerializeField] private Color negativeColor = Color.red;
        [SerializeField] private float showDuration = 1f;
        
        private List<Image> _images;
        private Image _frameImage;
        private void Awake()
        {
            _images = new List<Image>(frame.GetComponentsInChildren<Image>());
            _frameImage = frame.GetComponent<Image>();
            _images.Remove(_frameImage);
        }
        
        private void ShowFrame(bool show)
        {
            foreach (var image in _images)
            {
                image.color = show ? positiveColor : negativeColor;
            }
            frame.SetActive(true);
            Invoke(nameof(HideFrame), showDuration);
        }
        
        private void HideFrame()
        {
            frame.SetActive(false);
        }
        
        private void OnEnable()
        {
            playerEffectManager.onEffectApplied += ShowFrame;
        }
        
        private void OnDisable()
        {
            playerEffectManager.onEffectApplied -= ShowFrame;
        }
    }
}
