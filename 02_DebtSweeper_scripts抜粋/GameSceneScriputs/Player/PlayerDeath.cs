using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    private const string DEATH = "Death";
    private const string CHANGESCENENAME = "Level3";

    [SerializeField] private Animator _playerAnimator;

    private Camera _mainCamera;
    private Camera _overlookingCamera;
    private SceneChangeController _sceneChangeController;
    private bool _isPlayerDead = false;

    public void PlayerDeathStart(SceneChangeController sceneChangeController, Camera playerCamera, Camera subCamera)
    {
        _sceneChangeController = sceneChangeController;
        _mainCamera = playerCamera;
        _overlookingCamera = subCamera;
        _overlookingCamera.enabled = false;
    }

    /// <summary>
    /// プレイヤーが死んだ時の処理
    /// </summary>
    public void DeathPlayer()
    {
        _isPlayerDead = true; 
        _overlookingCamera.enabled = true;
        _mainCamera.enabled = false;
        _playerAnimator.SetTrigger(DEATH);
        _sceneChangeController.ChangeScene(CHANGESCENENAME);
    }

    /// <summary>
    /// Playerのステートを渡すためのメソッド
    /// </summary>
    /// <returns></returns>
    public bool IsPlayerDead()
    {
        return _isPlayerDead;
    }
}
