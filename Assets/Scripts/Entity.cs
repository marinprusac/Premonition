using UnityEngine;

public class Entity : MonoBehaviour
{
    
    public HexCoordinates Coordinates { get; private set; }
    private HexGridSettings _hexGridSettings;
    public int height = 1;
    public Tile StandingOnTile => HexGrid.Instance.GetTile(Coordinates + height * HexCoordinates.Down);
    

    private void Start()
    {
        _hexGridSettings = HexGrid.Instance.hexGridSettings;
        Coordinates = HexCoordinates.FromPixelCoordinates(transform.position, _hexGridSettings);
    }
    
    public void UpdateHexCoordinates()
    {
        Coordinates = HexCoordinates.FromPixelCoordinates(transform.position, _hexGridSettings);
    }
}
