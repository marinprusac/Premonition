using UnityEngine;

namespace Actors
{
    [RequireComponent(typeof(Actor))]
    public class Enemy : MonoBehaviour
    {
        [HideInInspector] public Actor actor;
    
        private void Awake()
        {
            actor = GetComponent<Actor>();
        }
    
    }
}