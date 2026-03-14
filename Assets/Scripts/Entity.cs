using UnityEngine;

public class Entity : MonoBehaviour
{
    
    public HexCoordinates Coordinates { get; private set; }
    private HexGridSettings _hexGridSettings;
    public int height = 1;

    public Tile StandingOnTile
    {
        get => HexGrid.Instance.GetTile(Coordinates + height * HexCoordinates.Down);
        
        set
        {
            var tileCoords = value.Coordinates;
            Coordinates = tileCoords + height * HexCoordinates.Up;
        }
    }

    private void Start()
    {
        _hexGridSettings = HexGrid.Instance.settings;
        Coordinates = HexCoordinates.FromPixelCoordinates(transform.position, _hexGridSettings);
    }
    
    public void UpdateHexCoordinates()
    {
        Coordinates = HexCoordinates.FromPixelCoordinates(transform.position, _hexGridSettings);
    }
}
