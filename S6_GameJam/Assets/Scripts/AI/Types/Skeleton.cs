using System.Collections.Generic;
using System.ComponentModel;
using KBCore.Refs;
using UnityEngine;

public class Skeleton : AIControllerBase
{
    private Dictionary<string, StateMachine.State> states;
    
    [SerializeField] private float _spawnDuration = 2.0f;
    [SerializeField] private float _attackDuration = 0.8f;
    [SerializeField] private float _killDuration = 3f;

    private Timer _timer;

    void TargetPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) ChangeTarget(player.transform.position, player);
    }

    void Start()
    {
        SubStart();
        states = InitStates();
        
        // State Handlers
        StateMachine.State spawningState = states["Spawning"];
        StateMachine.State marchingState = states["Marching"];
        StateMachine.State targetState = states["Target"];
        StateMachine.State attackState = states["Attacking"];
        StateMachine.State killedState = states["Killed"];
        _timer = new Timer(_spawnDuration, () => StateMachine.TransitionTo(marchingState));
        spawningState.OnFrame = () => _timer.Update(Time.deltaTime);

        // marchingState.OnEnter = () => ChangeTarget(baseTarget.transform.position);

        targetState.OnEnter = () => TargetPlayer();
        targetState.OnFrame = () => TargetPlayer();

        attackState.OnEnter = () =>
        {
            agent.speed = 0;
            Debug.Log("Attacking");
            _timer = new Timer(_attackDuration, () => StateMachine.TransitionTo(targetState));

            //TODO: Add attack animation and damage logic here
        };
        attackState.OnFrame = () => _timer.Update(Time.deltaTime);
        attackState.OnExit = () => agent.speed = speed;

        killedState.OnEnter = () =>
        {
            agent.speed = 0;
            Debug.Log("Killed");

            _timer = new Timer(_killDuration, () => Destroy(gameObject));
        };
        killedState.OnFrame = () => _timer.Update(Time.deltaTime);
    }

    void Update()
    {
        SubUpdate();
        // _timer.Update(Time.deltaTime);
    }
    
    void OnValidate() => this.ValidateRefs();
}
