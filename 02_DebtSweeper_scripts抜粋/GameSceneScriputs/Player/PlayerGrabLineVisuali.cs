using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlayerGrabLineVisuali : MonoBehaviour
{
    private const int CURVERESOLUTION = 20;// 曲線の分割数

    [SerializeField] private Transform _leftHand;// 左手のTransform
    
    private LineRenderer _line;

    void Start()
    {
        _line = GetComponent<LineRenderer>();
        _line.positionCount = CURVERESOLUTION;
        _line.material = new Material(Shader.Find("Sprites/Default"));
        _line.startWidth = 0.05f;
        _line.endWidth = 0.05f;
        _line.startColor = Color.yellow;
        _line.endColor = Color.yellow;
    }

    public void DrawLineController(ConfigurableJoint joint, Rigidbody objectRigidbody)
    {
        if (joint == null || objectRigidbody == null)
        {
            _line.enabled = false;
            return;
        }

        _line.enabled = true;

        Vector3 point0 = _leftHand.position;
        Vector3 point1 = joint.connectedAnchor;  // 接続点（ワールド座標）
        Vector3 point2 = objectRigidbody.position;

        DrawCurve(point0, point1, point2);
    }

    // Catmull-Rom スプラインで曲線を作る
    private void DrawCurve(Vector3 point0, Vector3 point1, Vector3 point2)
    {
        for (int i = 0; i < CURVERESOLUTION; i++)
        {
            float t = i / (CURVERESOLUTION - 1f);
            Vector3 point = QuadraticBezier(point0, point1, point2, t);
            _line.SetPosition(i, point);
        }
    }

    // 二次ベジェ曲線
    private Vector3 QuadraticBezier(Vector3 startingPoint, Vector3 directionPoint, Vector3 endPoint, float t)
    {
        return
            (1 - t) * (1 - t) * startingPoint +
            2 * (1 - t) * t * directionPoint +
            t * t * endPoint;
    }
}
