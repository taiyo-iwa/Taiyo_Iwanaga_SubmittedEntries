using UnityEngine;
using UnityEngine.UI;

public class PlayerObjectAcquisitionInView : MonoBehaviour
{
    private readonly Vector3 BOX_CHECK_SIZE = new Vector3(13.0f, 7.5f, 1.0f);
    private const float BOX_CHECK_DISTANCE = 7.0f;

    [SerializeField] LayerMask _canGrabObjectLayerMask = default;
    [SerializeField] private Camera _playerCamera = default;
    [SerializeField] private RectTransform _targetUI = default;
    [SerializeField] private RectTransform _parentUI = default;

    private void Update()
    {
        CameraInViewPlayer();
    }

    private void CameraInViewPlayer()
    {
        RaycastHit[] hitObjects = Physics.BoxCastAll(
            _playerCamera.transform.position,
            BOX_CHECK_SIZE * 0.5f,
            _playerCamera.transform.forward,
            _playerCamera.transform.rotation,
            BOX_CHECK_DISTANCE,
            _canGrabObjectLayerMask
        );

        if(hitObjects.Length <= 0)
        {
            _targetUI.gameObject.SetActive(false);
            return;
        }

        RaycastHit? nearestHit = null;
        float minDistance = float.MaxValue;
        foreach(RaycastHit hit in hitObjects)
        {
            float distance = hit.distance;
            if(distance < minDistance)
            {
                minDistance = distance;
                nearestHit = hit;
            }
        }

        ObjectMarker(nearestHit.Value.transform);
    }

    private void ObjectMarker(Transform targetTransform)
    {
        Transform cameraTransform = _playerCamera.transform;

        Vector3 cameraDirection = cameraTransform.transform.forward;

        Vector3 targetWorldPosition = targetTransform.position;

        Vector3 targetDirection = targetWorldPosition - cameraTransform.transform.position;

        bool isObjectFront = Vector3.Dot(cameraDirection, targetDirection) > 0;

        _targetUI.gameObject.SetActive(isObjectFront);

        if (!isObjectFront)
        {
            return; 
        }

        //ワールド座標をスクリーン座標に
        Vector3 targetScreenPosition = _playerCamera.WorldToScreenPoint(targetWorldPosition);

        //スクリーン座標からUIローカル座標変換
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentUI,
                                                                targetScreenPosition,
                                                                null, // オーバーレイモードの場合はnull
                                                                out Vector2 uiLocalPosition );

        _targetUI.localPosition = uiLocalPosition;
    }
}
