using UnityEngine;

public class MiniMapCameraFollow : MonoBehaviour
{
    [SerializeField] private MiniMapSettings _settings = default;
    [SerializeField] private float _cameraHeight = default;

    private void Awake()
    {
        _cameraHeight = transform.position.y;
    }

    private void Update()
    {
        Vector3 targetPosition = _settings._targetToFollow.position;

        transform.position = new Vector3(targetPosition.x, targetPosition.y +  _cameraHeight, targetPosition.z);

        if (_settings.rotateWithTheTarget)
        {
            Quaternion targetRotation = _settings._targetToFollow.transform.rotation;

            transform.rotation = Quaternion.Euler(90, targetRotation.eulerAngles.y, 0);
        }
    }
}
