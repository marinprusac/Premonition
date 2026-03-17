using UnityEngine;

namespace Grid
{
    [CreateAssetMenu(fileName = "HexGridSettings", menuName = "Hex Grid Settings", order = 0)]
    public class GridSettings : ScriptableObject
    {
        public float tileRadius;
        public float tileHeight;

    }
}