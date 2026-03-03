using System;
using KBCore.Refs;
using UnityEngine.AI;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;

    public Health()
    {
    }
    public Health(float maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
    }

    public Health(float maxHealth, float currentHealth)
    {
        this.maxHealth = maxHealth;
        this.currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public float GetMaxHealth() => maxHealth;
    public float GetCurrentHealth() => currentHealth;
    
    // Callbacks
    private Action<float> _onDamageTaken;
    private Action<float> _onHealed;
    private Action<float> _onDeath;
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            _onDeath?.Invoke(damage);
        }
        else
        {
            _onDamageTaken?.Invoke(damage);
        }
    }
    
    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        _onHealed?.Invoke(amount);
    }
    
    public void SetHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
    }
    public void SetHealth(float health, float maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
    }
    
    
    // Callback handlers
    public void RegisterOnDamageTaken(Action<float> callback) => _onDamageTaken += callback;
    public void UnregisterOnDamageTaken(Action<float> callback) => _onDamageTaken -= callback;
    public void RegisterOnHealed(Action<float> callback) => _onHealed += callback;
    public void UnregisterOnHealed(Action<float> callback) => _onHealed -= callback;
    public void RegisterOnDeath(Action<float> callback) => _onDeath += callback;
    public void UnregisterOnDeath(Action<float> callback) => _onDeath -= callback;
}
