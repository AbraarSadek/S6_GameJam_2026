using System.Collections.Generic;
using System.ComponentModel;
using KBCore.Refs;
using UnityEngine;

public class Skeleton : AIControllerBase
{
    private Dictionary<string, StateMachine.State> states;

    void Start()
    {
        SubStart();

        states = StateMachine.States;

    }

    void Update()
    {
        SubUpdate();
    }
    
    void OnValidate() => this.ValidateRefs();
}
