using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;

public class Skeleton : AIControllerBase
{
    private Dictionary<string, StateMachine.State> states;

    void Start()
    {
        states = InitStates();
    }
    
    void OnValidate() => this.ValidateRefs();
}
