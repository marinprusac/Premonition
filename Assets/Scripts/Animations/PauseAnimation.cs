using System;

namespace Animations
{
    public class PauseAnimation : Animation
    {
        public PauseAnimation(float duration, Action onCompleted = null) : base(duration, _ => { }, onCompleted)
        {
        }
    }
}