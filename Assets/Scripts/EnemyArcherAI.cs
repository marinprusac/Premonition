using System.Linq;
using UnityEngine;

[RequireComponent(typeof(EnemyArcher))]
public class EnemyArcherAI : MonoBehaviour, IActionDecider
{
    private Enemy _enemy;
    private EnemyArcher _archer;
    private ActionState _actionState;
    private Player Player => EntityManager.Instance.player;
    private Tile CurrentTile => _enemy.actor.entity.StandingOnTile;
    
    private MovementStats MovementStats => _enemy.actor.movementStats;
    private int Height => _enemy.actor.entity.height;
    
    private HexCoordinates Coordinates => _enemy.actor.entity.Coordinates;


    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _archer = GetComponent<EnemyArcher>();

    }
    
    private void StartedActing()
    {
        _actionState = ActionState.Acting;
    }
    
    private void DoneActing()
    {
        _actionState = ActionState.Ending;
    }

    private Tile GetBestShootingTile()
    {
        var reachableTiles = GridNavigation.GetAllReachableTilesWithinRange(CurrentTile, MovementStats, Height);

        
        var shootableTiles = reachableTiles.Where(t =>
            GridNavigation.IsEntityInWeaponsRange(t.CoordinatesWhenStandingOn(Height), Player.actor.entity, _archer.bow.weapon.stats)).ToList();

        if (shootableTiles.Count == 0) return null;
        
        
        var distances = shootableTiles.ConvertAll(t =>
            HexCoordinates.Distance(Player.actor.entity.Coordinates, t.CoordinatesWhenStandingOn(Height)));

        var farthestDistance = distances.Max();

        var farthestTile = shootableTiles.Find(t =>
            HexCoordinates.Distance(t.CoordinatesWhenStandingOn(Height), Player.actor.entity.Coordinates) ==
            farthestDistance);

        return farthestTile;
    }

    public ActionState ActionState => _actionState;

    public void MoveAction()
    {
        print("Enemy");
        StartedActing();
        var canSeePlayer = _enemy.CanSeeThePlayer;
        if (canSeePlayer)
        {
            var chosenTile = GetBestShootingTile();
            if (chosenTile is null)
            {
                DoneActing();
                return;
            }
            var path = GridNavigation.GetPath(CurrentTile, chosenTile, MovementStats, Height);
            _enemy.actor.Move(path, DoneActing);
        }
        else
        {
            DoneActing();
        }
    }

    public void AttackAction()
    {
        StartedActing();
        var playerInRange =
            GridNavigation.IsEntityInWeaponsRange(Coordinates, Player.actor.entity, _archer.bow.weapon.stats);
        
        if (playerInRange)
        {
            ActorAnimator.TurnTowards(_enemy.actor, Player.transform.position);
            ActorAnimator.TurnTowards(Player.actor, _enemy.transform.position);
            ActorAnimator.BraceShield(Player.shield);
            ActorAnimator.DrawBow(_archer.bow, Player.transform.position);
            ActorAnimator.LooseArrow(_archer.bow);
            ActorAnimator.UnbraceShield(Player.shield, DoneActing);
        }
        else
        {
            DoneActing();
        }
        
    }

    public void AcknowledgedEnd()
    {
        _actionState = ActionState.Waiting;
    }
}