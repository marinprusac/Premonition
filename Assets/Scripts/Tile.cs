using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tile : MonoBehaviour
{
    public HexCoordinates Coordinates { get; private set; }
    private Color _baseColor;
    private Material _ownMaterial;


    public void Highlight(Color color)
    {
        _ownMaterial.color = Color.Lerp(_baseColor, color, 0.35f);
    }
    
    
    public void ClearHighlight()
    {
        _ownMaterial.color =  _baseColor;
    }
    
    private void Awake()
    {
        var parent = transform.parent.GetComponentInParent<HexGrid>().hexGridSettings;
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
}