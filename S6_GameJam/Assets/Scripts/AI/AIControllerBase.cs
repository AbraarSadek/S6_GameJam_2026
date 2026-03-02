using System;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(Sight))]
public class AIControllerBase : MonoBehaviour
{
    [SerializeField, Self] protected StateMachine StateMachine;
    [SerializeField, Self] protected Sight sight;

    [SerializeField] private int health = 1;
    [SerializeField] private float speed = 3.5f;
    
        
    [SerializeField, Self] private NavMeshAgent agent;
    private Vector3 targetDestination;
    [SerializeField] private GameObject baseTarget;
    
    // States
    private StateMachine.State marchingState;
    private StateMachine.State targetState;
    private StateMachine.State killedState;

    protected Dictionary<string,StateMachine.State> InitStates()
    {
        StateMachine = new StateMachine();
        StateMachine.State spawningState = StateMachine.CreateState("Spawning"); // Initial State
        spawningState.OnEnter = () => agent.speed = 0;
        spawningState.OnExit = () => agent.speed = speed;
        marchingState = StateMachine.CreateState("Marching"); // Heading towards player base
        targetState = StateMachine.CreateState("Target"); // Heading towards object or player last known location
        StateMachine.State attackingState = StateMachine.CreateState("Attacking"); // Used when attacking an object or player
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
    
    protected void ChangeTarget(Vector3 newTarget)
    {
        targetDestination = newTarget;
        agent.destination = targetDestination;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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
    void Update()
    {
        // if (Vector3.Distance(transform.position, destination) < 1f)
        // {
        //     index = (index + 1) % waypoints.Length;
        //     destination = waypoints[index].transform.position;
        //     agent.destination = destination;
        // }
    }
}
