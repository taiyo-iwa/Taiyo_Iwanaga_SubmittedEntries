using UnityEngine;

public class MovieCameraController : MonoBehaviour
{
    [SerializeField] private Camera _camera = default;
    [SerializeField] private Vector3 _targetPosition = Vector3.zero;
    [SerializeField] private Quaternion _targetRotation = Quaternion.identity;
    [SerializeField] private float _positionMoveSpeed = 1.0f;
    [SerializeField] private float _rotationMoveSpeed = 1.0f;

    private void Update()
    {
        _camera.transform.position =  Vector3.Lerp(transform.position, _targetPosition, _positionMoveSpeed * Time.deltaTime);
        _camera.transform.rotation =  Quaternion.Lerp(transform.rotation, _targetRotation, _rotationMoveSpeed * Time.deltaTime);
    }
}
