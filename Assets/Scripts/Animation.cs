using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Animation
{
    public readonly float duration;
    private float _elapsedTime;
    private bool _toFinish => _elapsedTime >= duration;
    private Action _onCompleted;
    public bool Completed { get; private set; } = false;

    private FrameAction _action;
    
    public void Do(float deltaTime)
    {
        if (_toFinish)
        {
            if (Completed) return;
            _onCompleted?.Invoke();
            Completed = true;
            return;
        }
        _elapsedTime += deltaTime;
        _action?.Invoke(_elapsedTime / duration);
    }


    public delegate void FrameAction(float t);
    
    
    public Animation(float duration, FrameAction frameAction, Action onCompleted = null)
    {
        this.duration = duration;
        _action = frameAction;
        _onCompleted = onCompleted;
    }
    
}