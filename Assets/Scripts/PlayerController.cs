using System;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour, IActionDecider
{
    private Camera _camera;
    private Player _player;
    private MovementStats Stats => _player.actor.movementStats;
    public ActionState ActionState => _actionState;
    private ActionState _actionState = ActionState.Waiting;
    private State _state = State.Nothing;
    

    private enum State
    {
        Nothing,
        Moving,
        Attacking
    }
    
    
    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }
    
    private void Start()
    {
        _player = FindFirstObjectByType<Player>();
    }

    private void DoneActing()
    {
        _actionState = ActionState.Ending;
    }

    private T GetSelectedObject<T>() where T : MonoBehaviour
    {
        if (!Input.GetMouseButtonDown(0)) return null;
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return null;
        return hit.collider.GetComponent<T>();
    }
    

    public void MoveAction()
    {
        print("Player");
        var reachableTiles = GridNavigation.GetAllReachableTilesWithinRange(_player.actor.entity.StandingOnTile,
            _player.actor.movementStats, _player.actor.entity.height);
        Highlighter.Instance.HighlightTiles(reachableTiles);
        _state = State.Moving;
        _actionState = ActionState.Acting;
    }
    
    public void AttackAction()
    {
        _state = State.Attacking;
        _actionState = ActionState.Acting;

        var hittableEnemies =
            GridNavigation.GetAllEntitiesInWeaponsRange(_player.actor, _player.sword.weapon.stats);
        
        if (hittableEnemies.Count == 0)
        {
            _state = State.Nothing;
            DoneActing();
        }
        
        Highlighter.Instance.HighlightEntities(hittableEnemies);
    }

    public void AcknowledgedEnd()
    {
        _actionState = ActionState.Waiting;
    }

    private void TryMoving()
    {
        var selectedTile = GetSelectedObject<Tile>();
        if (!selectedTile) return;
        
        var reachableTiles = GridNavigation.GetAllReachableTilesWithinRange(_player.actor.entity.StandingOnTile, _player.actor.movementStats, _player.actor.entity.height);
        if (!reachableTiles.Contains(selectedTile)) return;
        _state = State.Nothing;
        var path = GridNavigation.GetPath(_player.actor.entity.StandingOnTile, selectedTile, _player.actor.movementStats, _player.actor.entity.height);
        Highlighter.Instance.ClearHighlights();
        _player.actor.Move(path, DoneActing);
    }

    private void TryAttacking()
    {
        var selectedEnemy = GetSelectedObject<Enemy>();
        if (!selectedEnemy) return;
        var enemyCoords = selectedEnemy.actor.entity.Coordinates;
        var playerCoords = _player.actor.entity.Coordinates;
        var heightDifference = selectedEnemy.actor.entity.height - _player.actor.entity.height;
        var distance = HexCoordinates.Distance(enemyCoords, playerCoords);
        if (heightDifference > Stats.climbHeight || heightDifference < -Stats.dropHeight || distance > 1) return;
        _state = State.Nothing;
        Highlighter.Instance.ClearHighlights();
        ActorAnimator.TurnTowards(_player.actor, selectedEnemy.transform.position);
        ActorAnimator.TurnTowards(selectedEnemy.actor, _player.transform.position);
        ActorAnimator.SwingSword(_player.actor, selectedEnemy.transform.position, _player.sword, DoneActing);
        //ActorAnimator.BraceShield(selectedEnemy.actor);
        //ActorAnimator.Unbrace(OnTurnFinished);
    }

    private void Update()
    {
        if (_actionState != ActionState.Acting) return;
        
        if (_state == State.Moving)
        {
            TryMoving();
        }
        else if (_state == State.Attacking)
        {
            TryAttacking();
        }
    }
}