using System;
using UnityEngine;


public class Timer<T>
{
    private float _duration;
    private float _elapsedTime;
    private Action<T> _callback;
    
    public Timer(float duration, Action<T> callback)
    {
        _duration = duration;
        _callback = callback;
        _elapsedTime = 0.0f;
    }
    
    public bool Update(float deltaTime, T param)
    {
        _elapsedTime += deltaTime;
        if (_elapsedTime >= _duration)
        {
            _callback?.Invoke(param);
            return true;
        }

        return false;
    }

    public void Reset()
    {
        _elapsedTime = 0.0f;
    }

    public void Reset(float duration)
    {
        _duration = duration;
        _elapsedTime = 0.0f;
    }
}

public class Timer
{
    private float _duration;
    private float _elapsedTime;
    private Action _callback;
    
    public Timer(float duration, Action callback)
    {
        _duration = duration;
        _callback = callback;
        _elapsedTime = 0.0f;
    }
    
    public bool Update(float deltaTime)
    {
        _elapsedTime += deltaTime;
        Debug.Log("Timer Update: " + _elapsedTime + " / " + _duration);
        
        if (_elapsedTime >= _duration)
        {
            _callback?.Invoke();
            return true;
        }

        return false;
    }

    public void Reset()
    {
        _elapsedTime = 0.0f;
    }

    public void Reset(float duration)
    {
        _duration = duration;
        _elapsedTime = 0.0f;
    }
}
