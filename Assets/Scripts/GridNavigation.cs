using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GridNavigation
{
    public static Tile[] GetNeighbors(Tile tile, int maxPositiveHeightDifference = 1, int maxNegativeHeightDifference = 1, int heightSpaceNeeded = 1)
    {
        var coords = tile.Coordinates;
        var neighbors = new List<Tile>();
        
        var directions = new []
        {
            HexCoordinates.BackLeft, HexCoordinates.Left, HexCoordinates.ForwardLeft, HexCoordinates.ForwardRight, HexCoordinates.Right, HexCoordinates.BackRight
        };

        var heightDifferences = new int[maxPositiveHeightDifference + 1 + maxNegativeHeightDifference];
        for (var i = 0; i < heightDifferences.Length; i++)
        {
            heightDifferences[i] = -maxNegativeHeightDifference + i;
        }
        
        foreach (var dir in directions)
        {
            var sameLevelNeighborCoords = coords + dir;
            
            foreach(var heightDifference in heightDifferences)
            {
                var neighborCoords = sameLevelNeighborCoords + new HexCoordinates(0, 0, 0, heightDifference);
                
                var neighboringTile = HexGrid.Instance.GetTile(neighborCoords);
                if (neighboringTile is null) continue;
                
                switch (heightDifference)
                {
                    case < 0 when !IsFreeSpace(sameLevelNeighborCoords, HexCoordinates.Up, heightSpaceNeeded, false):
                    case > 0 when !IsFreeSpace(coords + heightDifference * HexCoordinates.Up,HexCoordinates.Up, heightSpaceNeeded, false):
                    case >= 0 when !IsFreeSpace(neighborCoords, HexCoordinates.Up, heightSpaceNeeded, false):
                        continue;
                }

                if (EntityManager.Instance.IsTileOccupied(neighboringTile)) continue;
                
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
    
    public static Tile[] GetPath(Tile start, Tile end, MovementStats stats, int height)
    {
        var openSet = new List<Tile> { start };
        var cameFrom = new Dictionary<Tile, Tile>();
        var gScore = new Dictionary<Tile, int> { [start] = 0 };
        var fScore = new Dictionary<Tile, int> { [start] = HeuristicCostEstimate(start, end) };

        while (openSet.Count > 0)
        {
            var current = openSet.OrderBy(tile => fScore.ContainsKey(tile) ? fScore[tile] : int.MaxValue).First();

            if (current == end)
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);

            foreach (var neighbor in GetNeighbors(current, stats.climbHeight, stats.dropHeight, height))
            {
                var tentativeGScore = gScore[current] + 1;

                if (tentativeGScore < (gScore.ContainsKey(neighbor) ? gScore[neighbor] : int.MaxValue))
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, end);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return Array.Empty<Tile>();
    }
    
    public static bool IsFreeSpace(HexCoordinates origin, HexCoordinates direction, int spaces, bool includingOrigin = true)
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
            var tile = HexGrid.Instance.GetTile(coordinate);
            if (tile is not null) return false;
        }
        return true;
    }

    public static Tile[] GetAllReachableTilesWithinRange(Tile start, MovementStats stats, int height)
    {
        var visited = new HashSet<Tile>();
        var queue = new Queue<(Tile tile, int distance)>();
        queue.Enqueue((start, 0));
        visited.Add(start);

        while (queue.Count > 0)
        {
            var (currentTile, distance) = queue.Dequeue();

            if (distance < stats.movementRange)
            {
                foreach (var neighbor in GetNeighbors(currentTile, stats.climbHeight,
                             stats.dropHeight, height))
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue((neighbor, distance + 1));
                    }
                }
            }
        }

        return visited.ToArray();
    }
}