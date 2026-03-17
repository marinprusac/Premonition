using System;
using System.Linq;
using Actors;
using Animations;
using Grid;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Actions
{
    public class EnemyArcherAI : Controller
    {
        private Actor PlayerActor => ActorManager.Instance.playerActor;
    
        private bool GetPlayerVisible(Actor actor)
        {
            return NavigationServer.IsEntityVisible(actor.Coordinates, ActorManager.Instance.playerActor);
        }
        

        private Tile GetBestShootingTile(Actor actor)
        {
            var reachableTiles = NavigationServer.GetAllReachableTilesWithinRange(actor);

        
            var shootableTiles = reachableTiles.Where(t =>
                NavigationServer.IsActorInWeaponsRange(t.CoordinatesWhenStandingOn(actor.actorStats.height), PlayerActor, actor.equippedWeapon.stats)).ToList();

            if (shootableTiles.Count == 0) return null;
        
        
            var distances = shootableTiles.ConvertAll(t =>
                HexCoordinates.Distance(PlayerActor.Coordinates, t.CoordinatesWhenStandingOn(actor.actorStats.height)));

            var farthestDistance = distances.Max();

            var farthestTile = shootableTiles.Find(t =>
                HexCoordinates.Distance(t.CoordinatesWhenStandingOn(actor.actorStats.height), PlayerActor.Coordinates) ==
                farthestDistance);

            return farthestTile;
        }

        /// <summary>
        /// This function will return a random tile in range, prioritizing tiles that are higher up.
        /// </summary>
        private Tile ChooseTileToWander(Actor actor)
        {

            if (Random.value < 0.5f) return actor.StandingOnTile;
            
            var reachableTiles = NavigationServer.GetAllReachableTilesWithinRange(actor);
            if (reachableTiles.Count == 0) return null;
        
            var heights = reachableTiles.ConvertAll(t => t.Coordinates.h);
            var minHeight = heights.Min();
            var priorities = heights.ConvertAll(h => h - minHeight + 1);
            var total = priorities.Sum();
            var randomNumber = Random.Range(0, total);

            var i = 0;
            for (; i < priorities.Count; i++)
            {
                if (randomNumber <= 0) break;
                randomNumber -= priorities[i];
            }

            if (i >= reachableTiles.Count) i = reachableTiles.Count - 1;
            return reachableTiles[i];
        }


        public override void MoveAction(Actor actor, Action onDone)
        {
            Tile[] path;
            var knowsWhereThePlayerIs = GetPlayerVisible(actor);
            if (knowsWhereThePlayerIs)
            {
                var chosenTile = GetBestShootingTile(actor) ?? ChooseTileToWander(actor);
                if (chosenTile is null)
                {
                    onDone();
                    return;
                }
                path = NavigationServer.GetPath(actor.StandingOnTile, chosenTile, actor.actorStats);
            }
            else
            {
                var chosenTile = ChooseTileToWander(actor);
                if (chosenTile is null)
                {
                    onDone();
                    return;
                }
                path = NavigationServer.GetPath(actor.StandingOnTile, chosenTile, actor.actorStats);
            }

            if (path is { Length: > 0 })
                actor.StandingOnTile = path[^1];
            
            
            CombatServer.Move(actor, path, onDone);
        }

        public override void AttackAction(Actor actor, Action onDone)
        {
            var playerInRange =
                NavigationServer.IsActorInWeaponsRange(actor.Coordinates, PlayerActor, actor.equippedWeapon.stats);
        
            if (playerInRange)
            {
                CombatServer.Attack(actor, PlayerActor,  Mathf.Max(actor.currentEnergy-1, 0), onDone);
            }
            else
            {
                onDone();
            }
        
        }
        
    }
}