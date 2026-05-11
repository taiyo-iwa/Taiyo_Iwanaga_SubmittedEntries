using UnityEngine;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

public class RedemptionAnimationController : MonoBehaviour
{
    private const string STARTEXCHANGE = "StartExchange";
    private const float ANIMATIONWAITTIME = 1.0f;

    [SerializeField] private Animator _exchangeAnimator = default;
    [SerializeField] private GameObject _goalArea = default;

    private CancellationTokenSource _waitCts;

    public async UniTask StartCashExchange(float waitSeconds, float redemptionItemPrice, float quota)
    {
        if (_waitCts != null)
        {
            return;
        }

        _waitCts = new CancellationTokenSource();
        CancellationToken token = _waitCts.Token;

        try
        {
            await UniTask.WaitForSeconds(waitSeconds, cancellationToken: token);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("待機がキャンセルされました");
            _waitCts = null;
            return;
        }

        //再度ノルマチェック
        if (redemptionItemPrice < quota)
        {
            Debug.Log("条件を満たさずキャンセル");
            _waitCts = null;  //キャンセル不能化
            return;
        }

        _waitCts = null;  // キャンセル用CTS破棄（キャンセル不能化）

        _exchangeAnimator.SetTrigger(STARTEXCHANGE);

        await UniTask.WaitForSeconds(ANIMATIONWAITTIME);

        Debug.Log($"換金結果{redemptionItemPrice}");
        _goalArea.SetActive(true);
    }

    public void CancelWaiting()
    {
        _waitCts?.Cancel();
    }
}
