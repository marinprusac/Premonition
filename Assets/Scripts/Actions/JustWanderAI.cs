using System;
using Actors;
using Animations;
using Grid;
using Random = UnityEngine.Random;

namespace Actions
{
    public class JustWanderAI : Controller
    {
        public override void MoveAction(Actor actor, Action onDone)
        {
           var reachableTiles = NavigationServer.GetAllReachableTilesWithinRange(actor);
           var randomTile = Random.Range(0, reachableTiles.Count);
           var tile = reachableTiles[randomTile];
           var path = NavigationServer.GetPath(actor.StandingOnTile, tile, actor.actorStats);
           actor.StandingOnTile = path[^1];
           ActorAnimator.Move(actor, path, onDone);
        }

        public override void AttackAction(Actor actor, Action onDone)
        {
            onDone();
        }
    }
}