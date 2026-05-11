using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SceneChangeController : MonoBehaviour
{
    public async void ChangeScene(string changeSceneName)
    {
        await SceneChangeWait(changeSceneName);
    }

    private async UniTask SceneChangeWait(string changeSceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(changeSceneName);

        asyncLoad.allowSceneActivation = false;

        //移動するシーンがロードし終わるまで待つ
        while(asyncLoad.progress < 0.9f)
        {
            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        await UniTask.WaitForSeconds(2);

        asyncLoad.allowSceneActivation = true;

        await asyncLoad.ToUniTask();
    }
}
