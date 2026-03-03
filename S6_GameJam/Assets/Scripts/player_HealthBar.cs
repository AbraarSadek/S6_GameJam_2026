using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KBCore.Refs;

[RequireComponent(typeof(Health))]
public class player_HealthBar : MonoBehaviour {

    [SerializeField] private Image _healthbarSprite;
    [SerializeField] private float _reduceSpeed = 2f;
    
    [SerializeField, Self] private Health _health;

    private float _target = 1;

    public void UpdatePlayerHealthBar(float currentHealth, float maxHealth) {

        _target = currentHealth / maxHealth;

    }

    public void Update() {

        _healthbarSprite.fillAmount = Mathf.MoveTowards(_healthbarSprite.fillAmount, _target, _reduceSpeed * Time.deltaTime);
    }

    public void Start()
    {
        _health.RegisterOnHealed(_ =>
        {
            UpdatePlayerHealthBar(_health.GetCurrentHealth(), _health.GetMaxHealth());
        });
        _health.RegisterOnDamageTaken(_ =>
        {
            UpdatePlayerHealthBar(_health.GetCurrentHealth(), _health.GetMaxHealth());
        });
    }

}
