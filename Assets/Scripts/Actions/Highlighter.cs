using System.Collections.Generic;
using Actors;
using Grid;
using UnityEngine;

namespace Actions
{
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

        public void HighlightActors(List<Actor> actors) 
        {
            foreach (var actor in actors)
            {
                var material = actor.GetComponentInChildren<MeshRenderer>().material;
                _highlightedEntities[material] = material.color;
            }
        }

        private void Update()
        {
            foreach (var (material, baseColor) in _highlightedEntities)
            {
                material.color = Color.Lerp(baseColor, Color.red, (Mathf.Sin(Time.time*6)+1)/2);
            }
        
            foreach (var (material, baseColor) in _highlightedTiles)
            {
                material.color = Color.Lerp(baseColor, Color.white, (Mathf.Sin(Time.time*4)+1)/4);
            }
        }
    
    }
}