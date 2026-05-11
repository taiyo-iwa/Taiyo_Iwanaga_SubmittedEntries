using UnityEngine;

public class PlayerObjectGrab : MonoBehaviour
{
    private const float GRABDISTANCE = 3f;
    private const float GRABDISTANCE_ADDITIONAL_RATE = 0.5f;
    private const float MAXGRABDISTANCE = 5.0f;
    private const float MINGRABDISTANCE = 1.5f;
    private const float POSITIONSPRING = 200f;
    private const float POSITIONDAMPER = 30f;
    private const float MAXFORCE = 800f;

    [SerializeField] LayerMask _canGrabObjectLayerMask = default;

    private Camera _playerCamera = default;
    private PlayerMousePointerChange _mousePointerChange = default;
    private PlayerGrabLineVisuali _lineVisuali = default;  
    private Rigidbody _objectRigidbody = default;
    private PickupableItem _pickupableItem = default;
    private ConfigurableJoint _joint = default;
    private float _grabDistance = 3.0f;
    private bool _isGrabbedObject = false;

    private void Update()
    {          
        if (_joint != null)
        {
            MoveGrabPoint();
        }

        if (_isGrabbedObject)
        {
            _mousePointerChange.GrabbedPointerSprite();
        }
        else
        {
            MousePointerController();
        }

        _lineVisuali.DrawLineController(_joint, _objectRigidbody);
    }

    public void PlayerObjectGrabStart(PlayerMousePointerChange playerMousePointerChange, PlayerGrabLineVisuali playerGrabLineVisuali, Camera playerCamera)
    {
        _mousePointerChange = playerMousePointerChange;
        _lineVisuali = playerGrabLineVisuali;
        _playerCamera = playerCamera;
    }

    public void ChangeGrabDistance(float MouseScroll)
    {
        _grabDistance += MouseScroll * GRABDISTANCE_ADDITIONAL_RATE;
        _grabDistance = Mathf.Clamp(_grabDistance, MINGRABDISTANCE, MAXGRABDISTANCE);
    }

    public void TryGrab()
    {
        Ray ray = _playerCamera.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, GRABDISTANCE, _canGrabObjectLayerMask))
        {
            return;
        }
        
        _objectRigidbody = hit.rigidbody;
        _pickupableItem = hit.collider.GetComponent<PickupableItem>();
        if (_objectRigidbody == null)
        {
            return;         
        }
        if(_pickupableItem == null)
        {
            return;
        }

        _isGrabbedObject = true;

        _joint = _objectRigidbody.gameObject.AddComponent<ConfigurableJoint>();
        _joint.autoConfigureConnectedAnchor = false;

        // 掴む位置（ワールド座標）
        _joint.connectedAnchor = ray.GetPoint(_grabDistance);

        // X/Y/Z 方向をバネで拘束
        JointDrive drive = new JointDrive
        {
            positionSpring = POSITIONSPRING,
            positionDamper = POSITIONDAMPER,
            maximumForce = MAXFORCE
        };

        _joint.xDrive = drive;
        _joint.yDrive = drive;
        _joint.zDrive = drive;
        
        // 回転はフリー
        _joint.angularXMotion = ConfigurableJointMotion.Free;
        _joint.angularYMotion = ConfigurableJointMotion.Free;
        _joint.angularZMotion = ConfigurableJointMotion.Free;
        
        // 位置合わせモード
        _joint.configuredInWorldSpace = true;
    }

    private void MoveGrabPoint()
    {
        Ray ray = _playerCamera.ScreenPointToRay(Input.mousePosition);
        _joint.connectedAnchor = ray.GetPoint(_grabDistance);
    }

    public void Release()
    {     
        if (_joint != null)
        {
            Destroy(_joint);
        }
        _isGrabbedObject = false;
        _grabDistance = 3.0f;
        _objectRigidbody = null;
        _pickupableItem = null;
    }

    private void MousePointerController()
    {
        Ray ray = _playerCamera.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, GRABDISTANCE, _canGrabObjectLayerMask))
        {
            _mousePointerChange.ChangePointerSprite(false);
            return;
        }

        _objectRigidbody = hit.rigidbody;
        _pickupableItem = hit.collider.GetComponent<PickupableItem>();
        if (_objectRigidbody == null)
        {
            return;
        }
        if (_pickupableItem == null)
        {
            return;
        }

        _mousePointerChange.ChangePointerSprite(true);
    }
}
