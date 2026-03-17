using System;
using System.Collections.Generic;
using Actors;
using Unity.VisualScripting;
using UnityEngine;

namespace Actions
{
    public class TurnManager : MonoBehaviour
    {
        public static TurnManager Instance { get; private set; }
        
        public enum CombatPhase
        {
            NotStarted,
            Moving,
            Attacking,
            Ended
        }

        private enum TurnPhase
        {
            Waiting,
            Acting
        }

        public CombatPhase combatPhase = CombatPhase.NotStarted;
        private TurnPhase _turnPhase = TurnPhase.Waiting;
        private readonly List<Actor> _actors = new();
        private int _index;
        public int turnCount;
        
        public Actor CurrentlyActing => _actors[_index];


        private readonly List<Actor> _spawnWaitingList = new();


        private void AddToSpawnWaitingList(Actor actor)
        {
            _spawnWaitingList.Add(actor);
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            _actors.AddRange(FindObjectsByType<Actor>(FindObjectsSortMode.None));
            combatPhase = CombatPhase.Moving;
        }


        private void OnTurnDone()
        {
            _index++;

            if (_index >= _actors.Count)
            {
                _index = 0;
                if (combatPhase == CombatPhase.Moving)
                {
                    combatPhase = CombatPhase.Attacking;
                }
                else
                {
                    combatPhase = CombatPhase.Moving;
                    _actors.AddRange(_spawnWaitingList);
                    _spawnWaitingList.Clear();
                    _actors.Reverse();



                    var gameOver = true;
                    foreach (var enemy in ActorManager.Instance.enemyActors)
                    {
                        if (enemy.alive)
                        {
                            gameOver = false;
                            break;
                        };
                    }

                    if (gameOver)
                    {
                        combatPhase = CombatPhase.Ended;
                        return;
                    }
                    turnCount++;
                    
                }
            }
            _turnPhase = TurnPhase.Waiting;
        }

        private void Update()
        {
            if (combatPhase is CombatPhase.NotStarted or CombatPhase.Ended) return;
            if (_turnPhase == TurnPhase.Acting) return;
            _turnPhase = TurnPhase.Acting;
            var actor = _actors[_index];
            if (!actor.alive)
            {
                OnTurnDone();
                return;
            }
            if (combatPhase == CombatPhase.Moving)
            {
                actor.currentEnergy = (int)MathF.Min(actor.actorStats.maxEnergy, actor.currentEnergy + actor.actorStats.energyRegeneration);
                actor.controller.MoveAction(actor, OnTurnDone);
            }
            else
            {
                actor.currentEnergy = (int)MathF.Min(actor.actorStats.maxEnergy, actor.currentEnergy + actor.actorStats.energyRegeneration);
                actor.controller.AttackAction(actor, OnTurnDone);
            }
        }
    }
}