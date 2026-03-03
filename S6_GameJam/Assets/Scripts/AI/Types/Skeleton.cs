using System.Collections.Generic;
using System.ComponentModel;
using KBCore.Refs;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Skeleton : AIControllerBase
{
    private Dictionary<string, StateMachine.State> states;
    
    [Header("Animation Settings")]
    [SerializeField, Self] private Animator _animator;
    
    

    void Start()
    {
        SubStart();

        states = StateMachine.States;

        states["Marching"].OnEnter += () => _animator.SetBool("Walking", true);
        states["Marching"].OnExit += () => _animator.SetBool("Walking", false);
        states["Target"].OnEnter += () => _animator.SetBool("Walking", true);
        states["Target"].OnExit += () => _animator.SetBool("Walking", false);
        states["Searching"].OnEnter += () => _animator.SetBool("Walking", true);
        states["Searching"].OnExit += () => _animator.SetBool("Walking", false);
        // states["Attacking"].OnEnter += () => _animator.SetBool("Attacking", true);
        // states["Attacking"].OnExit += () => _animator.SetBool("Attacking", false);
        onAttackHit += () => _animator.SetTrigger("Attack");
        states["Killed"].OnEnter += () => _animator.SetBool("Dead", true);
    }

    void Update()
    {
        SubUpdate();
    }
    
    void OnValidate() => this.ValidateRefs();
}
