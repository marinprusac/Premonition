using System;
using System.Collections.Generic;
using System.Linq;
using Actors;
using Items;
using UnityEngine;

namespace Grid
{
    public static class NavigationServer
    {
        private static Tile[] GetNeighbors(Tile tile, ActorStats actorStats)
        {
            var coords = tile.Coordinates;
            var neighbors = new List<Tile>();
        
            var directions = new []
            {
                HexCoordinates.BackLeft, HexCoordinates.Left, HexCoordinates.ForwardLeft, HexCoordinates.ForwardRight, HexCoordinates.Right, HexCoordinates.BackRight
            };

            var heightDifferences = new int[actorStats.climbHeight + 1 + actorStats.dropHeight];
            for (var i = 0; i < heightDifferences.Length; i++)
            {
                heightDifferences[i] = -actorStats.dropHeight + i;
            }
        
            foreach (var dir in directions)
            {
                var sameLevelNeighborCoords = coords + dir;
            
                foreach(var heightDifference in heightDifferences)
                {
                    var neighborCoords = sameLevelNeighborCoords + new HexCoordinates(0, 0, 0, heightDifference);
                
                    var neighboringTile = GridManager.Instance.GetTile(neighborCoords);
                    if (neighboringTile is null) continue;
                
                    switch (heightDifference)
                    {
                        case < 0 when !IsFreeSpace(sameLevelNeighborCoords, HexCoordinates.Up, actorStats.height, false):
                        case > 0 when !IsFreeSpace(coords + heightDifference * HexCoordinates.Up,HexCoordinates.Up, actorStats.height, false):
                        case >= 0 when !IsFreeSpace(neighborCoords, HexCoordinates.Up, actorStats.height, false):
                            continue;
                    }

                    if (IsTileOccupied(neighboringTile)) continue;

                    if (!IsFreeSpace(neighboringTile.Coordinates + HexCoordinates.Up, HexCoordinates.Zero, 1)) continue;
                
                    neighbors.Add(neighboringTile);
                }
            }
            return neighbors.ToArray();
        }
    
    
        private static int HeuristicCostEstimate(Tile a, Tile b)
        {
            var dq = Math.Abs(a.Coordinates.q - b.Coordinates.q);
            var dr = Math.Abs(a.Coordinates.r - b.Coordinates.r);
            var ds = Math.Abs(a.Coordinates.s - b.Coordinates.s);
            var dh = Math.Abs(a.Coordinates.h - b.Coordinates.h);
            return dq + dr + ds + dh;
        }
    
        private static Tile[] ReconstructPath(Dictionary<Tile, Tile> cameFrom, Tile current)
        {
            var totalPath = new List<Tile> { current };
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.Add(current);
            }
            totalPath.Reverse();
            return totalPath.ToArray();
        }
    
        public static Tile[] GetPath(Tile start, Tile end, ActorStats stats)
        {
            if (end is null) return null;
            var openSet = new List<Tile> { start };
            var cameFrom = new Dictionary<Tile, Tile>();
            var gScore = new Dictionary<Tile, int> { [start] = 0 };
            var fScore = new Dictionary<Tile, int> { [start] = HeuristicCostEstimate(start, end) };

            while (openSet.Count > 0)
            {
                var current = openSet.OrderBy(tile => fScore.GetValueOrDefault(tile, int.MaxValue)).First();

                if (current == end)
                {
                    return ReconstructPath(cameFrom, current);
                }

                openSet.Remove(current);

                foreach (var neighbor in GetNeighbors(current, stats))
                {
                    var tentativeGScore = gScore[current] + 1;

                    if (tentativeGScore >= (gScore.GetValueOrDefault(neighbor, int.MaxValue))) continue;
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, end);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }

            return null;
        }

        public static Tile[] GetPathToNeighboringTile(Actor actor, Tile end)
        {
            var start = actor.StandingOnTile;
            var stats = actor.actorStats;
            var reachableTiles = GetAllReachableTilesWithinRange(actor);
            
            var neighbors = GetNeighbors(end, stats);
            Tile[] shortestPath = null;
            foreach (var neighbor in neighbors)
            {
                var pathToNeighbor = GetPath(start, neighbor, stats);
                if (pathToNeighbor is null) continue;
                if (shortestPath is null || pathToNeighbor.Length < shortestPath.Length)
                    shortestPath = pathToNeighbor;
            }

            if (shortestPath is null || shortestPath.Length <= 1) return null;
            var availableEnergy = actor.currentEnergy;
            var availableDistance = availableEnergy / 2;
            var pathDistance = shortestPath.Length-1;
            var dist = Mathf.Min(availableDistance, pathDistance);
            return shortestPath[..(dist+1)];

        }

        private static bool IsFreeSpace(HexCoordinates origin, HexCoordinates direction, int spaces, bool includingOrigin = true)
        {
            int i = 0;
            if (!includingOrigin)
            {
                i = 1;
                spaces++;
            }
        
            for(;  i < spaces; i++)
            {
                var coordinate = origin + i * direction;
                var tile = GridManager.Instance.GetTile(coordinate);
                if (tile is not null) return false;
            }
            return true;
        }

        public static List<Tile> GetAllReachableTilesWithinRange(Actor actor)
        {
            var start = actor.StandingOnTile;
            var stats = actor.actorStats;
            var energy = actor.currentEnergy;
            var visited = new HashSet<Tile>();
            var queue = new Queue<(Tile tile, int distance)>();
            queue.Enqueue((start, 0));
            visited.Add(start);

            while (queue.Count > 0)
            {
                var (currentTile, distance) = queue.Dequeue();
                foreach (var neighbor in GetNeighbors(currentTile, stats))
                {
                    var newDist = distance + 1;
                    if (newDist> stats.movementRange || newDist * 2 > energy) continue;
                    if (!visited.Add(neighbor)) continue;
                    queue.Enqueue((neighbor, newDist));
                }
            }
        
            return visited.ToList();
        }

        private static bool IsTileOccupied(Tile tile)
        {
            var tileCoords = tile.Coordinates;
            var occupied = ActorManager.Instance.actors.Any(actor => actor.Coordinates.Equals2D(tileCoords) && actor.StandingOnTile == tile);
            return occupied;
        }

        public static bool IsEntityVisible(HexCoordinates from, Actor target)
        {
            var fromPos = from.ToPixelCoordinates(GridManager.Instance.settings);
            var toPos = target.transform.position;
            var direction = toPos - fromPos;
            if (Physics.Raycast(fromPos + direction.normalized, direction, out var hit, direction.magnitude))
            {
                return hit.collider.gameObject == target.gameObject;
            }
            return true;
        }
    
        public static bool IsActorInWeaponsRange(HexCoordinates from, Actor target, WeaponStats weapon)
        {
            if(!IsEntityVisible(from, target)) return false;
            var distance = HexCoordinates.Distance(from, target.Coordinates);
            var heightDistance = Mathf.Abs((target.Coordinates - from).h);
            var combinedDistance = distance + heightDistance / 2f;
            
            return weapon.minRange <= combinedDistance && combinedDistance <= weapon.maxRange;
        }

        public static List<Actor> GetAllEntitiesInWeaponsRange(Actor from, WeaponStats weapon)
        {
            var actors = ActorManager.Instance.actors;
            actors = actors.Where(a => a.alive).ToList();
            var inRange = actors.Where(actor => IsActorInWeaponsRange(from.Coordinates, actor, weapon)).ToList();
            inRange.Remove(from);
            return inRange;
        }
    }
}