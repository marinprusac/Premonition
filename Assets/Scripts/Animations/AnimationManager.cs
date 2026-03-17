using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animations
{
    public class AnimationManager : MonoBehaviour
    {
    
        private readonly List<Animation> _animationQueue = new();
        private bool _playingAnimation;
    
        public static AnimationManager Instance { get; private set; }
    
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        
        }

        private void Update()
        {
            if (_playingAnimation || _animationQueue.Count <= 0) return;
            _playingAnimation = true;
            var anim = _animationQueue[0];
            _animationQueue.RemoveAt(0);
            StartCoroutine(PerformAnimation(anim));
        }

        public void QueueAnimation(Animation anim)
        {
            _animationQueue.Add(anim);
        }

        private IEnumerator PerformAnimation(Animation a)
        {
            while (!a.Completed)
            {
                a.Do(Time.deltaTime);
                yield return null;
            }
            _playingAnimation = false;
        }
    
    
        
    }
}