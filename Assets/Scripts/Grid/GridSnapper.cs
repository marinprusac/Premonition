using UnityEngine;

namespace Grid
{
    [ExecuteInEditMode]
    public class GridSnapper : MonoBehaviour
    {
        private const float WaitTime = 0.1f;
        private float _waitCounter = WaitTime;
        private Vector3 _lastPosition = Vector3.zero;
        private GridSettings _gridSettings;
    
        [SerializeField] private GridSettings gridSettingsOverride;
        
        private void Update()
        {
            _gridSettings ??= gridSettingsOverride ?? GetComponentInParent<GridManager>().settings;
        
            if (Vector3.Distance(transform.position, _lastPosition) > 0.01f)
            {
                _waitCounter = WaitTime;
                _lastPosition = transform.position;
                return;
            }

            _waitCounter -= Time.deltaTime;
            if (_waitCounter > 0) return;

            _waitCounter = WaitTime;
            var hex = HexCoordinates.FromPixelCoordinates(transform.position, _gridSettings);
            transform.position = hex.ToPixelCoordinates(_gridSettings);
        
            _lastPosition = transform.position;
        }


    }
}