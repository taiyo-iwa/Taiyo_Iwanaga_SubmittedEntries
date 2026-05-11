using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    [SerializeField] private string _changeSceneName = default;

    public async void ChangeScene()
    {
        await SceneChangeWait();
    }

    private async UniTask SceneChangeWait()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_changeSceneName);

        asyncLoad.allowSceneActivation = false;

        //移動するシーンがロードし終わるまで待つ
        while (asyncLoad.progress < 0.9f)
        {
            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        await UniTask.WaitForSeconds(2);

        asyncLoad.allowSceneActivation = true;

        await asyncLoad.ToUniTask();
    }
}
