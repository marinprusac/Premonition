using System;

namespace Animations
{
    public class EmptyAnimation : InstantAnimation
    {
        public EmptyAnimation(Action onCompleted = null) : base(() => {}, onCompleted)
        {
        }
    }
}