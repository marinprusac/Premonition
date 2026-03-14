using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public static ActionManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private enum CombatPhase
    {
        NotStarted,
        Moving,
        Attacking,
    }

    private CombatPhase _phase = CombatPhase.NotStarted;
    
    private readonly List<IActionDecider> _turnOrder = new();
    private int _currentActorIndex = 0;

    private void Start()
    {
        var player = FindAnyObjectByType<PlayerController>();
        var enemyArchers = FindObjectsByType<EnemyArcherAI>(FindObjectsSortMode.None);

        _turnOrder.Add(player);
        foreach (var enemy in enemyArchers)
        {
            _turnOrder.Add(enemy);
        }
        
        _phase = CombatPhase.Moving;
    }

    private void Update()
    {
        if(_phase == CombatPhase.NotStarted) return;
        
        var currentActor = _turnOrder[_currentActorIndex];

        switch (currentActor.ActionState)
        {
            case ActionState.Waiting:
                switch (_phase)
                {
                    case CombatPhase.Moving:
                        currentActor.MoveAction();
                        break;
                    case CombatPhase.Attacking:
                        currentActor.AttackAction();
                        break;
                    case CombatPhase.NotStarted:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                break;
            case ActionState.Ending:
            {
                currentActor.AcknowledgedEnd();
                _currentActorIndex++;
                if (_currentActorIndex < _turnOrder.Count) return;
                _phase = _phase switch
                {
                    CombatPhase.Moving => CombatPhase.Attacking,
                    CombatPhase.Attacking => CombatPhase.Moving,
                    _ => _phase
                };
                _currentActorIndex = 0;
                if(_phase == CombatPhase.Moving) _turnOrder.Reverse(); 
                break;
            }
            case ActionState.Acting:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }
}