using System;

namespace Animations
{
    public class Animation
    {
        private readonly float _duration;
        private float _elapsedTime;
        private bool ToFinish => _elapsedTime >= _duration;
        private readonly Action _onCompleted;
        public bool Completed { get; private set; } = false;

        private readonly FrameAction _action;
    
        public void Do(float deltaTime)
        {
            if (ToFinish)
            {
                if (Completed) return;
                _onCompleted?.Invoke();
                Completed = true;
                return;
            }
            _elapsedTime += deltaTime;
            _action?.Invoke(_elapsedTime / _duration);
        }


        public delegate void FrameAction(float t);
    
    
        public Animation(float duration, FrameAction frameAction, Action onCompleted = null)
        {
            this._duration = duration;
            _action = frameAction;
            _onCompleted = onCompleted;
        }
    
    }
}