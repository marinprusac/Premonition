using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Actors
{
    public class ActorManager : MonoBehaviour
    {
        [HideInInspector] public List<Actor> actors;
        [HideInInspector] public Actor playerActor;
        [HideInInspector] public List<Actor> enemyActors;

        public static ActorManager Instance;
    
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            actors = FindObjectsByType<Actor>(FindObjectsSortMode.None).ToList();
            playerActor = FindAnyObjectByType<Player>().GetComponent<Actor>();
            enemyActors = FindObjectsByType<Enemy>(FindObjectsSortMode.None).ToList().ConvertAll(e => e.GetComponent<Actor>());
        }
    

    }
}