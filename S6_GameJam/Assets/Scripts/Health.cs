using System;
using KBCore.Refs;
using UnityEngine.AI;
using UnityEngine;

public class Health
{
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _currentHealth = 100f;
    
    public float GetMaxHealth() => _maxHealth;
    public float GetCurrentHealth() => _currentHealth;
    
    // Callbacks
    private Action<float> _onDamageTaken;
    private Action<float> _onHealed;
    private Action<float> _onDeath;
    
    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
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
        _currentHealth += amount;
        if (_currentHealth > _maxHealth) _currentHealth = _maxHealth;
        _onHealed?.Invoke(amount);
    }
    
    public void SetHealth(float health)
    {
        _currentHealth = Mathf.Clamp(health, 0, _maxHealth);
    }
    public void SetHealth(float health, float maxHealth)
    {
        this._maxHealth = maxHealth;
        _currentHealth = Mathf.Clamp(health, 0, maxHealth);
    }
    
    
    // Callback handlers
    public void RegisterOnDamageTaken(Action<float> callback) => _onDamageTaken += callback;
    public void UnregisterOnDamageTaken(Action<float> callback) => _onDamageTaken -= callback;
    public void RegisterOnHealed(Action<float> callback) => _onHealed += callback;
    public void UnregisterOnHealed(Action<float> callback) => _onHealed -= callback;
    public void RegisterOnDeath(Action<float> callback) => _onDeath += callback;
    public void UnregisterOnDeath(Action<float> callback) => _onDeath -= callback;
}
