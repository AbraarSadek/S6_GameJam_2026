using System;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Sight))]
[RequireComponent(typeof(Health))]
public abstract class AIControllerBase : MonoBehaviour
{
    protected StateMachine StateMachine;
    [SerializeField, Self] protected Sight sight;
    [SerializeField, Self] protected NavMeshAgent agent;

    [SerializeField, Self] protected Health health;
    [SerializeField] protected float speed = 3.5f;
    
    
    private Vector3 _targetDestination;
    private bool playerTargeted = false;
    private bool beenAtTarget;
    [SerializeField] protected GameObject baseTarget;
    
    // Attack hitbox
    [SerializeField] protected Collider attackHitbox;
    
    // States
    private StateMachine.State spawningState;
    private StateMachine.State marchingState;
    private StateMachine.State targetState;
    private StateMachine.State searchingState;
    private StateMachine.State killedState;
    private StateMachine.State attackingState;
    
    [SerializeField] private float _spawnDuration = 2.0f;
    [SerializeField] private float _attackDuration = 0.8f;
    [SerializeField] private float _killDuration = 3f;
    
    private Timer _timer;

    protected bool atTarget()
    {

        return Vector3.Distance(transform.position, _targetDestination) < agent.stoppingDistance;
    }

    private void InitStates()
    {
        StateMachine = new StateMachine();
        spawningState = StateMachine.CreateState("Spawning"); // Initial State
        marchingState = StateMachine.CreateState("Marching"); // Heading towards player base
        targetState = StateMachine.CreateState("Target"); // Heading towards object
        searchingState = StateMachine.CreateState("Searching"); // Heading to last known location of player after losing sight
        attackingState = StateMachine.CreateState("Attacking"); // Used when attacking an object or player
        killedState = StateMachine.CreateState("Killed"); // Used when the AI is killed.
    }

    // private void OnCollisionEnter(Collision other)
    // {
    //     if (other.gameObject.CompareTag("Snowball"))
    //     {
    //         
    //     }
    // }
    
    protected void ChangeTarget(Vector3 newTarget, bool isPlayer = false)
    {
        _targetDestination = newTarget;
        agent.destination = _targetDestination;
        beenAtTarget = false;
        if (isPlayer)
        {
            playerTargeted = true;
        }
        agent.stoppingDistance = 1f;
    }
    
    private void TargetPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector3 playerPos = player.transform.position;
            ChangeTarget(playerPos, true);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void SubStart()
    {
        InitStates();
        
        if (baseTarget == null)
        {
            baseTarget = GameObject.FindGameObjectWithTag("Base");
        }
        ChangeTarget(baseTarget.transform.position);
        
        health.RegisterOnDeath(_ => StateMachine.TransitionTo(killedState));
        
        sight.SetOnPlayerDetected(() => StateMachine.TransitionTo(targetState));
        sight.SetOnPlayerLost(() => StateMachine.TransitionTo(searchingState));
        
        // State Handlers
        // Spawning state will allow execution of spawn animation, hold the AI in place then transition to marching
        _timer = new Timer(_spawnDuration, () => StateMachine.TransitionTo(marchingState));
        spawningState.OnEnter = () => agent.speed = 0;
        spawningState.OnExit = () => agent.speed = speed;
        spawningState.OnFrame = () => _timer.Update(Time.deltaTime);
        
        // Marching state will march towards the player base unless a player or object is targeted.
        marchingState.OnEnter = () => ChangeTarget(baseTarget.transform.position);
        marchingState.AtDestination = () => StateMachine.TransitionTo(attackingState);
        
        // Target state will continue to target the player until they are lost, then transition to searching
        targetState.OnEnter = () => TargetPlayer();
        targetState.OnFrame = () => TargetPlayer();
        targetState.AtDestination = () => StateMachine.TransitionTo(attackingState);
        
        // Searching state will continue to the last known location of the player, if the player is not found then it will transition back to marching
        searchingState.OnEnter = () =>
        {
            if (beenAtTarget)
            {
                agent.speed = 0;
                // Spin around looking for player for a few seconds, if not found transition back to marching
                _timer = new Timer(3f, () =>
                {
                    if (StateMachine.currentState == searchingState) StateMachine.TransitionTo(marchingState);
                });
            }
            else
            {
                // De-agro timer
                _timer = new Timer(20f, () =>
                {
                    if (StateMachine.currentState == searchingState) StateMachine.TransitionTo(marchingState);
                });
            }
        };
        searchingState.OnExit = () => agent.speed = speed;
        searchingState.OnFrame = () =>
        {
            _timer.Update(Time.deltaTime);

            if (beenAtTarget)
            {
                float rotationSpeed = 90f; // degrees per second
                transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
            }
        };
        searchingState.AtDestination = () =>
        {
            agent.speed = 0;
            // Spin around looking for player for a few seconds, if not found transition back to marching
            _timer = new Timer(3f, () =>
            {
                if (StateMachine.currentState == searchingState) StateMachine.TransitionTo(marchingState);
            });
        };
        
        
        // Attacking state will stop the AI and play attack animation, then transition back to marching or target depending on if the player is still targeted
        attackingState.OnEnter = () =>
        {
            agent.speed = 0;
            Debug.Log("Attacking");
            _timer = new Timer(_attackDuration, () =>
            {
                if (StateMachine.currentState != attackingState) return;
                
                // Using the hitbox, find all colliders in the area and apply damage to the player or base if they are hit
                Collider[] hitColliders = Physics.OverlapBox(attackHitbox.bounds.center, attackHitbox.bounds.extents, attackHitbox.transform.rotation);
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.gameObject == gameObject) continue; // Don't hit self
                    Health otherHealth = hitCollider.GetComponent<Health>();
                    if (otherHealth != null)
                    {
                        Debug.Log($"Dealt damage to {hitCollider.gameObject.name}");
                        otherHealth.TakeDamage(20f); // Example damage value
                    }
                }

                if (atTarget())
                {
                    _timer.Reset();
                }
                else if (sight.playerDetected)
                {
                    StateMachine.TransitionTo(targetState);
                }
                else
                {
                    StateMachine.TransitionTo(searchingState);
                }
            });

            //TODO: Add attack animation
        };
        attackingState.OnFrame = () => _timer.Update(Time.deltaTime);
        attackingState.OnExit = () => agent.speed = speed;

        killedState.OnEnter = () =>
        {
            agent.speed = 0;
            Debug.Log("Killed");

            _timer = new Timer(_killDuration, () => Destroy(gameObject));
        };
        killedState.OnFrame = () => _timer.Update(Time.deltaTime);
    }

    void OnValidate() => this.ValidateRefs();

    // Update is called once per frame
    protected void SubUpdate()
    {
        StateMachine.Update();
        
        if (atTarget() && !beenAtTarget)
        {
            Debug.Log("Reached target");
            beenAtTarget = true;
            StateMachine.InvokeAtDestination();
        }
    }
}
