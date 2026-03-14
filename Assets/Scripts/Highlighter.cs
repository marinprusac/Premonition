using System;
using System.Collections.Generic;
using UnityEngine;

public class Highlighter : MonoBehaviour
{
    
    public static Highlighter Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    private readonly Dictionary<Material, Color> _highlightedTiles = new();
    private readonly Dictionary<Material, Color> _highlightedEntities = new();
    
    public void ClearHighlights()
    {
        
        foreach (var (material, baseColor) in _highlightedTiles)
        {
            material.color = baseColor;
        }

        foreach (var (material, baseColor) in _highlightedEntities)
        {
            material.color = baseColor;
        }
        
        _highlightedTiles.Clear();
        _highlightedEntities.Clear();
    }
    
    public void HighlightTiles(List<Tile> tiles)
    {
        foreach (var tile in tiles)
        {
            var material = tile.GetComponent<MeshRenderer>().material;
            _highlightedTiles[material] = material.color;
        }

    }

    public void HighlightEntities(List<Entity> entities)
    {
        foreach (var entity in entities)
        {
            var material = entity.GetComponent<MeshRenderer>().material;
            _highlightedEntities[material] = material.color;
        }
    }

    private void Update()
    {
        foreach (var (material, baseColor) in _highlightedEntities)
        {
            material.color = Color.Lerp(baseColor, Color.white, (Mathf.Sin(Time.time*4)+1)/4);
        }
        
        foreach (var (material, baseColor) in _highlightedTiles)
        {
            material.color = Color.Lerp(baseColor, Color.white, (Mathf.Sin(Time.time*4)+1)/4);
        }
    }
    
}