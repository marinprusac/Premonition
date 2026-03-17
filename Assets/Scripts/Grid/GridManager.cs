using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Grid
{
    public class GridManager : MonoBehaviour
    {
    
    
    
        public static GridManager Instance { get; private set; }

        public GridSettings settings;
    
        private Dictionary<HexCoordinates, Tile> _tileDictionary;
    
        public Tile[] Tiles => _tileDictionary.Values.ToArray();

        [CanBeNull]
        public Tile GetTile(HexCoordinates coordinates)
        {
            _tileDictionary.TryGetValue(coordinates, out var tile);
            return tile;
        
        }


        public void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void Start()
        {
            var tiles = GetComponentsInChildren<Tile>();
            _tileDictionary = new Dictionary<HexCoordinates, Tile>(tiles.Length);
            foreach (var tile in tiles)
            {
                _tileDictionary[tile.Coordinates] = tile;
            }
        }


    
    
    }
}
