using System;

namespace Animations
{
    public class InstantAnimation : Animation
    {
        public InstantAnimation(Action action, Action onCompleted = null) : base(float.Epsilon, _ => action(), onCompleted)
        {
        }
    }
}