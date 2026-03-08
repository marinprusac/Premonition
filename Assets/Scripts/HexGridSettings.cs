using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "HexGridSettings", menuName = "Hex Grid Settings", order = 0)]
public class HexGridSettings : ScriptableObject
{
    public float tileRadius = 0;
    public float tileHeight = 0;

    public bool IsConfigured => tileRadius > 0 && tileHeight > 0;
}