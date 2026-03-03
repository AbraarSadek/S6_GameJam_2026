using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class player_HealthBar : MonoBehaviour {

    [SerializeField] private Image _healthbarSprite;
    [SerializeField] private float _reduceSpeed = 2f;

    private float _target = 1;

    public void UpdatePlayerHealthBar(float currentHealth, float maxHealth) {

        _target = currentHealth / maxHealth;

    }

    public void Update() {

        _healthbarSprite.fillAmount = Mathf.MoveTowards(_healthbarSprite.fillAmount, _target, _reduceSpeed * Time.deltaTime);

    }

}
