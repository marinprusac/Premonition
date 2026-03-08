using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
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
    
    private Player _player;

    
    
    public bool readyToMove = false;
    
    private void Start()
    {
        _player = FindFirstObjectByType<Player>();
        TileHighlighter.Instance.HighlightReachableTiles(_player.movableEntity);
        readyToMove = true;
    }

    private void OnAnimationFinished()
    {
        readyToMove = true;
        TileHighlighter.Instance.HighlightReachableTiles(_player.movableEntity);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && readyToMove)
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                _selectedTile = hit.collider.GetComponent<Tile>();
                if (_selectedTile is null) return;
                var reachableTiles = GridNavigation.GetAllReachableTilesWithinRange(_player.movableEntity.entity.StandingOnTile, _player.movableEntity.movementStats, _player.movableEntity.entity.height);
                if (!reachableTiles.Contains(_selectedTile)) return;
                var path = GridNavigation.GetPath(_player.movableEntity.entity.StandingOnTile, _selectedTile, _player.movableEntity.movementStats, _player.movableEntity.entity.height);
                readyToMove = false;
                TileHighlighter.Instance.ClearHighlightedTiles();
                _player.movableEntity.MoveAnimated(path, OnAnimationFinished);
                
            }
            else
            {
                _selectedTile = null;
            }
        }
        
        
        
        
    }
}