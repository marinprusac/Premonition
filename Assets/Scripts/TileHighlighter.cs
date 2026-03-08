using System;
using System.Collections.Generic;
using UnityEngine;

public class TileHighlighter : MonoBehaviour
{
    
    public static TileHighlighter Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    private readonly List<Tile> _highlightedTiles = new();
    
    public void HighlightReachableTiles(MovableEntity movableEntity)
    {
        ClearHighlightedTiles();
        var startingTile = movableEntity.entity.StandingOnTile;
        var reachableTiles = GridNavigation.GetAllReachableTilesWithinRange(startingTile, movableEntity.movementStats, movableEntity.entity.height);
        
        foreach (var reachableTile in reachableTiles)
        {
             reachableTile.Highlight(Color.cadetBlue);
             _highlightedTiles.Add(reachableTile);
        }
    }

    public void ClearHighlightedTiles()
    {
        foreach (var tile in _highlightedTiles)
        {
            tile.ClearHighlight();
        }
        _highlightedTiles.Clear();
    }
    
}