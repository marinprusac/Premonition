using Items;
using UnityEngine;

namespace Actors
{
    [RequireComponent(typeof(Actor))]
    public class Player : MonoBehaviour
    {
        [HideInInspector] public Actor actor;
        
        [HideInInspector] public int turnsTakes = 0;
        [HideInInspector] public int damageTaken;
        
    
        private void Awake()
        {
            actor = GetComponent<Actor>();
        }
    
    
    
    }
}