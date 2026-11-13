using System;
using RacingGame.RacingEffects;
using UnityEngine;
using UnityEngine.UI;

namespace RacingGame.UI
{
    public class ItemImage : MonoBehaviour
    {
        [SerializeField] private EffectManager playerEffectManager;
        private Image _image;
        
        private void Awake()
        {
            _image = GetComponent<Image>();
        }
        
        private void UpdateImage(RacingEffect effect)
        {
            _image.sprite = effect.Icon;
        }

        private void OnEnable()
        {
            playerEffectManager.onEffectChanged += UpdateImage;
        }
        
        private void OnDisable()
        {
            playerEffectManager.onEffectChanged -= UpdateImage;
        }
    }
}
