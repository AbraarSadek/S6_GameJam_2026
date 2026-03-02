using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sense : MonoBehaviour
{
    public bool bDebug = true;
    
    public Aspect.Affiliation targetAffiliation = Aspect.Affiliation.Enemy;
    public float detectionRate = 1.0f;
    
    protected float elapsedTime = 0.0f;
    protected virtual void Initialize() { }
    protected virtual void UpdateSense() { }
    
    private void Start()
    {
        Initialize();
    }
    
    private void Update()
    {
        UpdateSense();
    }
}
