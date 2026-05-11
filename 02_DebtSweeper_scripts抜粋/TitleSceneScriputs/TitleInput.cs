using UnityEngine;

public class TitleInput : MonoBehaviour
{
    [SerializeField] string _titleName;
    [SerializeField] private SceneChangeController _sceneChangeController = default;
    private bool _isMoveTrigger = true;

    private void Update()
    {
        if (Input.anyKeyDown && _isMoveTrigger)
        {
            _isMoveTrigger = false;
            _sceneChangeController.ChangeScene(_titleName);
        }
    }
}
