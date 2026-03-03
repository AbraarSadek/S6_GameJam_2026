using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class house_HealthBar : MonoBehaviour {

    [SerializeField] private Image _healthbarSprite;
    [SerializeField] private float _reduceSpeed = 2f;

    private float _target = 1;

    [SerializeField] private Health _houseHealth;

    public void UpdateHouseHealthBar(float currentHealth, float maxHealth) {

        _target = currentHealth / maxHealth;

    }

    public void Update() {

        _healthbarSprite.fillAmount = Mathf.MoveTowards(_healthbarSprite.fillAmount, _target, _reduceSpeed * Time.deltaTime);

    }

    public void Start()
    {
        _houseHealth.RegisterOnHealed(_ =>
        {
            UpdateHouseHealthBar(_houseHealth.GetCurrentHealth(), _houseHealth.GetMaxHealth());
        });
        _houseHealth.RegisterOnDamageTaken(_ =>
        {
            UpdateHouseHealthBar(_houseHealth.GetCurrentHealth(), _houseHealth.GetMaxHealth());
        });
    }

}
