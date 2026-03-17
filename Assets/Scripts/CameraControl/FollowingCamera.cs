using Actors;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace CameraControl
{
    public class FollowingCamera : MonoBehaviour
    {
        [SerializeField] private Actor target;
        private float _distanceOffset;
        private float _heightOffset;
        private float _startingHeight;
        private float _startingDistance;
        private float _targetAngle;
        private const float LerpFactor = 0.05f;

    
        private float _zoom = 1f;

        private Vector3 _baseRotation;

        private Vector3 _followPosition;
    
        [SerializeField] private float minZoom = 0.5f;
        [SerializeField] private float maxZoom = 2f;
    
    
        private void Start()
        {
        
            _startingHeight = transform.position.y - target.transform.position.y;
            _heightOffset = _startingHeight;
            _startingDistance = Vector3.Distance(transform.position, target.transform.position);
            _distanceOffset = _startingDistance;
            _baseRotation = transform.rotation.eulerAngles;
            _targetAngle = _baseRotation.y;
            _followPosition = target.transform.position;
        }

        private void Update()
        {
        
            var targetPositionNew = target.transform.position;
            _followPosition =
                Vector3.Lerp(_followPosition, targetPositionNew, 1 - Mathf.Pow(LerpFactor, Time.deltaTime));
            var moveTo = _followPosition - Quaternion.Euler(0, _targetAngle, 0) * Vector3.forward * _distanceOffset + Vector3.up * _heightOffset;
            transform.position = moveTo;

            if (Input.GetMouseButton(1))
            {
                var mouseDeltaX = Input.GetAxis("Mouse X");
                _targetAngle += mouseDeltaX;
            }

            _zoom = Mathf.Clamp(_zoom * (1 - Input.mouseScrollDelta.y/10f), minZoom, maxZoom);
            _heightOffset = _startingHeight * _zoom;
            _distanceOffset = _startingDistance * _zoom;

            transform.rotation = Quaternion.Euler(_baseRotation.x, _targetAngle, _baseRotation.z);
        }
        
    }
}