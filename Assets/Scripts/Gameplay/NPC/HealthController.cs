using GolfCourse.UI;
using UnityEngine;

namespace GolfCourse.NPC
{
    public class HealthController : MonoBehaviour
    {
        #region Fields

        [Header("Health Settings")]
        [SerializeField]
        private float maxHealth = 100f;

        [SerializeField]
        private float currentHealth;

        [SerializeField]
        private float healthDecreasingWeight = 1f;

        [Header("Health Bar UI")]
        [SerializeField]
        private HealthBarUI healthBarUI;

        private bool _isInitialized = false;
        #endregion

        #region Properties

        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;

        #endregion

        #region Events

        public delegate void HealthChanged(float currentHealth, float maxHealth);
        public event HealthChanged OnHealthChanged;

        public delegate void HealthDepleted();
        public event HealthDepleted OnHealthDepleted;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            OnHealthDepleted += HandleHealthDepleted;
        }

        private void OnDisable()
        {
            OnHealthDepleted -= HandleHealthDepleted;
        }

        private void Update()
        {
            if (!_isInitialized)
                return;
            DecreaseHealth(Time.deltaTime * healthDecreasingWeight);
        }

        #endregion

        #region Public Methods

        public void Initialize()
        {
            
            currentHealth = maxHealth;
            healthBarUI.InitializeHealthBar(this, currentHealth, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            _isInitialized = true;
        }

        public void DecreaseHealth(float amount)
        {
            if (currentHealth <= 0) return;

            currentHealth -= amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            healthBarUI.UpdateHealthBar(currentHealth, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (currentHealth == 0)
            {
                OnHealthDepleted?.Invoke();
            }
        }

        #endregion

        #region Private Methods

        private void HandleHealthDepleted()
        {
            Debug.Log($"{gameObject.name} has been incapacitated due to zero health.");
        }

        #endregion
    }
}
