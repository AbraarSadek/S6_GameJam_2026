using System;
using System.Collections.Generic;
using System.ComponentModel;
using KBCore.Refs;
using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Sight))]
public class AIControllerBase : MonoBehaviour
{
    [SerializeField] protected StateMachine StateMachine;
    [SerializeField, Self] protected Sight sight;
    [SerializeField, Self] protected NavMeshAgent agent;

    [SerializeField] protected int health = 1;
    [SerializeField] protected float speed = 3.5f;
    
    
    private Vector3 _targetDestination;
    private bool playerTargeted = false;
    [SerializeField] protected GameObject baseTarget;
    
    
    // States
    private StateMachine.State spawningState;
    private StateMachine.State marchingState;
    private StateMachine.State targetState;
    private StateMachine.State killedState;
    private StateMachine.State attackingState;

    protected Dictionary<string,StateMachine.State> InitStates()
    {
        StateMachine = new StateMachine();
        spawningState = StateMachine.CreateState("Spawning"); // Initial State
        spawningState.OnEnter = () => agent.speed = 0;
        spawningState.OnExit = () => agent.speed = speed;
        marchingState = StateMachine.CreateState("Marching"); // Heading towards player base
        targetState = StateMachine.CreateState("Target"); // Heading towards object or player last known location
        attackingState = StateMachine.CreateState("Attacking"); // Used when attacking an object or player
        killedState = StateMachine.CreateState("Killed"); // Used when the AI is killed.
        return StateMachine.States;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Snowball"))
        {
            StateMachine.TransitionTo(killedState);
        }
    }
    
    protected void ChangeTarget(Vector3 newTarget, bool isPlayer = false)
    {
        _targetDestination = newTarget;
        agent.destination = _targetDestination;
        if (isPlayer)
        {
            playerTargeted = true;
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void SubStart()
    {
        sight.SetOnPlayerDetected(() => StateMachine.TransitionTo(targetState));
        sight.SetOnPlayerLost(() => StateMachine.TransitionTo(marchingState));
        
        if (baseTarget == null)
        {
            baseTarget = GameObject.FindGameObjectWithTag("Base");
        }
        ChangeTarget(baseTarget.transform.position);
    }

    void OnValidate() => this.ValidateRefs();

    // Update is called once per frame
    protected void SubUpdate()
    {
        StateMachine.Update();
        if (Vector3.Distance(transform.position, _targetDestination) < 1f)
        {
            Debug.Log("Reached target");
            if (playerTargeted && StateMachine.currentState == targetState) // Check still targeting player
            {
                StateMachine.TransitionTo(attackingState);
            }
            else
            {
                ChangeTarget(baseTarget.transform.position);
            }
        }
    }
}
