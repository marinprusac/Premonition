using System;
using Actors;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Actions
{
    public class GameOverChecker : MonoBehaviour
    {

        public static GameOverChecker Instance;
        
        private void Start()
        {
            if (Instance) return;
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (ended) return;
            ended = TurnManager.Instance.combatPhase is TurnManager.CombatPhase.Ended;
            if (ended)
            {
                finalScore = TurnManager.Instance.turnCount + ActorManager.Instance.playerActor.damageTaken;
                SceneManager.LoadScene(2);
            }
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public int finalScore = 0;
        public bool ended = false;
    }
}