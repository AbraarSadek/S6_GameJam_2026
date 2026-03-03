using System.Collections.Generic;
using UnityEngine;
using KBCore.Refs;

[RequireComponent(typeof(Attack))]
public class Snowman : AIControllerBase
{
    private Dictionary<string, StateMachine.State> states;
    [SerializeField, Self] Attack attackComponent;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SubStart();
        
        states = StateMachine.States;

        onAttackHit += () =>
        {
            attackComponent.Throw();
        };
    }

    // Update is called once per frame
    void Update()
    {
        SubUpdate();
    }
    
    void OnValidate() => this.ValidateRefs();
}
