using System;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class GetTileInfo : MonoBehaviour
{
    private Camera _camera;


    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }


    private Tile _selectedTile = null;
    public int maxPositiveHeightDifference = 1;
    public int maxNegativeHeightDifference = 1;
    public int heightSpaceNeeded = 1;
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            foreach (var tile in HexGrid.Instance.Tiles)
            {
                tile.GetComponent<MeshRenderer>().material.color = Color.white;
            }
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                _selectedTile = hit.collider.GetComponent<Tile>();
                if (_selectedTile is null) return;
                var material = hit.collider.GetComponent<MeshRenderer>().material;
                print(_selectedTile.Coordinates);
                Tile[] reachableTiles = null;//GridNavigation.GetAllReachableTilesWithinRange(_selectedTile, 2, maxPositiveHeightDifference, maxNegativeHeightDifference, heightSpaceNeeded);
                foreach (var tile in reachableTiles)
                {
                    tile.GetComponent<MeshRenderer>().material.color = Color.lightBlue;
                }
                material.color = Color.lightSkyBlue;
                
            }
            else
            {
                _selectedTile = null;
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (_selectedTile is null) return;
            foreach (var tile in HexGrid.Instance.Tiles)
            {
                tile.GetComponent<MeshRenderer>().material.color = Color.white;
            }
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit)) return;
            
            var targetTile = hit.collider.GetComponent<Tile>();
            if (targetTile is null) return;
            _selectedTile.GetComponent<MeshRenderer>().material.color = Color.lightSkyBlue;
            targetTile.GetComponent<MeshRenderer>().material.color = Color.lightCoral;
            Tile[] path = null;// GridNavigation.GetPath(_selectedTile, targetTile, maxPositiveHeightDifference, maxNegativeHeightDifference, heightSpaceNeeded);
            print(path.Length);
            if (path.Length <= 2) return;
            foreach (var tile in path[1..^1])
            {
                tile.GetComponent<MeshRenderer>().material.color = Color.antiqueWhite;
            }
        }
        
        
        
        
    }
}