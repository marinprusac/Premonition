using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tile : MonoBehaviour
{
    public HexCoordinates Coordinates { get; private set; }
    private Color _baseColor;
    private Material _ownMaterial;
    
    
    private void Awake()
    {
        var parent = transform.parent.GetComponentInParent<HexGrid>().settings;
        Coordinates = HexCoordinates.FromPixelCoordinates(transform.position, parent);
    }

    private void Start()
    {
        _ownMaterial = GetComponent<MeshRenderer>().material;
        Color.RGBToHSV(_ownMaterial.color, out var h, out var s, out var v);

        v += Coordinates.h / 10f;
        s += Random.value / 10f;

        _baseColor = Color.HSVToRGB(h, s, v);
        _ownMaterial.color = _baseColor;
    }

    public HexCoordinates CoordinatesWhenStandingOn(int height)
    {
        return Coordinates + new HexCoordinates(0, 0, 0, height);
    }
}