using System;
using UnityEngine;

[ExecuteInEditMode]
public class HexSnapper : MonoBehaviour
{
    
    
    private const float WaitTime = 0.1f;
    private float _waitCounter = WaitTime;
    private Vector3 _lastPosition = Vector3.zero;
    private HexGridSettings _hexGridSettings;
    
    [SerializeField] private HexGridSettings hexGridSettingsOverride;
    

    private void Update()
    {
        
        _hexGridSettings ??= hexGridSettingsOverride ?? GetComponentInParent<HexGrid>().hexGridSettings;
        
        if (Vector3.Distance(transform.position, _lastPosition) > 0.01f)
        {
            _waitCounter = WaitTime;
            _lastPosition = transform.position;
            return;
        }

        _waitCounter -= Time.deltaTime;
        if (_waitCounter > 0) return;

        _waitCounter = WaitTime;
        var hex = HexCoordinates.FromPixelCoordinates(transform.position, _hexGridSettings);
        transform.position = hex.ToPixelCoordinates(_hexGridSettings);
        
        _lastPosition = transform.position;
    }


}