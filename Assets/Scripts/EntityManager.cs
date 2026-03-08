using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    private List<Entity> _entities;

    public static EntityManager Instance;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _entities = FindObjectsByType<Entity>(FindObjectsSortMode.None).ToList();
    }
    
    public bool IsTileOccupied(Tile tile)
    {
        var tileCoords = tile.Coordinates;
        var occupied = _entities.Any(e => e.Coordinates.Equals2D(tileCoords) && e.StandingOnTile == tile);
        return occupied;
    }
}