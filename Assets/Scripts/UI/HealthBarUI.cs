
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GolfCourse.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        #region Properties

        public RectTransform RectTransform => _rectTransform;

        #endregion

        #region Fields

        [Header("Health Bar UI")] [SerializeField]
        private Canvas _healthBarCanvas;

        [SerializeField] private Slider _healthSlider;
        [SerializeField] private Image _fillImage;
        [SerializeField] private TMP_Text _healthText;

        [Header("Health Bar Settings")] [SerializeField]
        private Gradient _healthGradient;

        [SerializeField] private RectTransform _rectTransform;
        private HealthController _healthController;

        #endregion

        #region Public Methods

        public void InitializeHealthBar(HealthController controller, float currentHealth, float maxHealth)
        {
            _healthController = controller;

            _healthController.OnHealthChanged += UpdateHealthBar;
            _healthBarCanvas.worldCamera = Camera.main;
            _healthBarCanvas.transform.localRotation = Quaternion.identity;

            _healthSlider.maxValue = maxHealth;
            _healthSlider.value = currentHealth;
            _healthText.text = $"{currentHealth:F0} / {maxHealth}";
        }

        public void UpdateHealthBar(float currentHealth, float maxHealth)
        {
            float healthPercentage = currentHealth / maxHealth;
            _healthSlider.value = currentHealth;
            _healthText.text = $"{currentHealth:F0} / {maxHealth}";
            _fillImage.color = _healthGradient.Evaluate(healthPercentage);
        }

        #endregion
    }
}