using GolfCourse.UI;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float _healthDecreasingWeight;

    [Header("Health Bar UI")]
    [SerializeField] private HealthBarUI healthBarUI;

    // Events
    public delegate void HealthChanged(float currentHealth, float maxHealth);
    public event HealthChanged OnHealthChanged;

    public delegate void HealthDepleted();
    public event HealthDepleted OnHealthDepleted;

    private void Update()
    {
        // Example: Decrease health over time
        DecreaseHealth(Time.deltaTime*_healthDecreasingWeight);
    }

    /// <summary>
    /// Initializes the health and health bar.
    /// </summary>
    public void Initialize()
    {
        currentHealth = maxHealth;
        healthBarUI.InitializeHealthBar(this, currentHealth, maxHealth);
    }

    /// <summary>
    /// Decreases health by a specified amount.
    /// </summary>
    /// <param name="amount">Amount to decrease.</param>
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
    
    /// <summary>
    /// Handles actions when health is depleted.
    /// </summary>
    private void HandleHealthDepleted()
    {
        Debug.Log($"{gameObject.name} has been incapacitated due to zero health.");
        // Additional logic if needed
    }

    private void OnEnable()
    {
        OnHealthDepleted += HandleHealthDepleted;
    }

    private void OnDisable()
    {
        OnHealthDepleted -= HandleHealthDepleted;
    }
}
