using UnityEngine;

[ExecuteInEditMode]
public class CameraPrePositioner : MonoBehaviour
{

    private Camera _camera;
    [SerializeField] private Transform target;
    [SerializeField] private float distanceOffset;
    [SerializeField] private float heightOffset;
    [SerializeField] private float rotationOffset;
    
    
        
    private void Update()
    {
        _camera ??= GetComponent<Camera>();
        if (_camera is null) return;
        transform.position = target.position + Quaternion.Euler(0, rotationOffset, 0) * Vector3.back * distanceOffset + Vector3.up * heightOffset;
        transform.rotation = Quaternion.Euler(30, rotationOffset, 0);
        transform.LookAt(target);
    }
}