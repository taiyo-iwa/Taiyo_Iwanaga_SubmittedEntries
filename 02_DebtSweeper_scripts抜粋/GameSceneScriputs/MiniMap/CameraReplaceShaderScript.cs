using UnityEngine;

public class CameraReplaceShaderScript : MonoBehaviour
{
    [SerializeField] private Camera _targetCamera;

    private void Start()
    {
        _targetCamera.SetReplacementShader(Shader.Find("Unlit/Color"), "RenderType");
    }
}
