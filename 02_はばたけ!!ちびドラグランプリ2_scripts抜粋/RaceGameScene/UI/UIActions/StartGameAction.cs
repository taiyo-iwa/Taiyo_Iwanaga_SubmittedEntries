using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGameAction : MonoBehaviour
{
    private const string LOADING = "Loading";
    private const float CHANGE_SCENE_WAIT_TIME = 1.0f;

    [SerializeField] private string _changeSceneName = default;
    [SerializeField] private AudioSource _audioSource = default;
    [SerializeField] private AudioClip _pressAudio = default;
    [SerializeField] private Animator _loadingAnimator = default;
    [SerializeField] private GameObject _loadingImage = default;
    [SerializeField] private GameObject _loadingScrollbar = default;

    private Scrollbar _scrollbar = default;
    private bool _isPressedFlag = false;

    private void Start()
    {
        _scrollbar = _loadingScrollbar.GetComponent<Scrollbar>();
        _loadingImage.SetActive(false);
        _loadingScrollbar.SetActive(false);
    }

    //UIのButtonのOnClickで発火
    public void OnStartButton()
    {
        if (!_isPressedFlag)
        {
            _isPressedFlag = true;
            ChangeScene();
            _audioSource.PlayOneShot(_pressAudio);
            //ロード画面表示
            _loadingImage.SetActive(true);
            _loadingScrollbar.SetActive(true);
            _loadingAnimator.SetBool(LOADING, true);
        }
    }

    public async void ChangeScene()
    {
        await SceneChangeWait();
    }

    //移動するシーンがロードされるまで待つ
    private async UniTask SceneChangeWait()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_changeSceneName);

        asyncLoad.allowSceneActivation = false;

        //移動するシーンがロードし終わるまで待つ
        while (asyncLoad.progress < 0.9f)
        {  
            await UniTask.Yield(PlayerLoopTiming.Update);
            float LoadValue = asyncLoad.progress / 0.9f;
            _scrollbar.size = LoadValue;
        }

        await UniTask.WaitForSeconds(CHANGE_SCENE_WAIT_TIME);

        asyncLoad.allowSceneActivation = true;

        await asyncLoad.ToUniTask();
    }
}
