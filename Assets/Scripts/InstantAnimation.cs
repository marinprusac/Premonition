using System;

public class InstantAnimation : Animation
{
    public InstantAnimation(Action action, Action onCompleted = null) : base(float.Epsilon, (t) => action(), onCompleted)
    {
    }
}