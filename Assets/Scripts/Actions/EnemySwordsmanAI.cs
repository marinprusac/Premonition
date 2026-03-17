using System;
using System.Linq;
using Actors;
using Animations;
using Grid;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Actions
{
    public class EnemySwordsmanAI : Controller
    {
        private Actor PlayerActor => ActorManager.Instance.playerActor;



        private int _visionCooldown;
        private Tile _lastKnownPlayerTile;
        
        
        private bool GetPlayerVisible(Actor actor)
        {
            return NavigationServer.IsEntityVisible(actor.Coordinates, ActorManager.Instance.playerActor);
        }
        
        private Tile[] GetPathLeadingToNeighboringTile(Actor actor, Tile tile)
        {
            
            var path = NavigationServer.GetPathToNeighboringTile(actor, tile);

            if (path == null) return null;
            return path.Length - 1 <= actor.actorStats.movementRange ? path : path[..(actor.actorStats.movementRange+1)];
        }

        private Tile GetWanderingTile(Actor actor)
        {
            var reachableTiles = NavigationServer.GetAllReachableTilesWithinRange(actor).ToList();

            if (reachableTiles.Count == 0)
            {
                return null;
            }
            
            var randomIndex = Random.Range(0, reachableTiles.Count);
            var tile = reachableTiles[randomIndex];
            return tile;
        }

        private void UpdateVisibility(Actor actor)
        {
            if (GetPlayerVisible(actor))
            {
                _visionCooldown = 2;
                _lastKnownPlayerTile = PlayerActor.StandingOnTile;
            }
            else if(_visionCooldown == 2)
            {
                _visionCooldown = 1;
                _lastKnownPlayerTile = PlayerActor.StandingOnTile;
            }
            else if (_visionCooldown == 1)
            {
                _visionCooldown = 0;
            }
        }

        public override void MoveAction(Actor actor, Action onDone)
        {
            UpdateVisibility(actor);
            var reachableActors = NavigationServer.GetAllEntitiesInWeaponsRange(actor, actor.equippedWeapon.stats);
            var playerVisible = GetPlayerVisible(actor);
            Tile[] path = null;
            
            if (reachableActors.Contains(PlayerActor))
            {
            }
            else if (playerVisible)
            { 
                path = GetPathLeadingToNeighboringTile(actor, PlayerActor.StandingOnTile);
                if (path == null)
                {
                    onDone();
                    return;
                }
            }
            else if (_lastKnownPlayerTile is not null
                     && HexCoordinates.Distance(actor.StandingOnTile.Coordinates, _lastKnownPlayerTile.Coordinates) > 1)
            {
                path = GetPathLeadingToNeighboringTile(actor, _lastKnownPlayerTile);
            }
            else
            {
                _lastKnownPlayerTile = null;
                var tile = GetWanderingTile(actor);
                path = NavigationServer.GetPath(actor.StandingOnTile, tile, actor.actorStats);
            }
            
            
            CombatServer.Move(actor, path, onDone);
        }

        public override void AttackAction(Actor actor, Action onDone)
        {
            UpdateVisibility(actor);
            var playerInWeaponsRange = NavigationServer.GetAllEntitiesInWeaponsRange(actor, actor.equippedWeapon.stats).Contains(PlayerActor);
            if (playerInWeaponsRange)
            {
                CombatServer.Attack(actor, PlayerActor, actor.currentEnergy, onDone);
            }
            else
            {
                onDone();
            }
        }
        
    }
}