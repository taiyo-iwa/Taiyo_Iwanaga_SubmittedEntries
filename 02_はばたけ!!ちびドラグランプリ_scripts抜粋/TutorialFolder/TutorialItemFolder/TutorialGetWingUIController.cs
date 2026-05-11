using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialGetWingUIController : MonoBehaviour
{
    //10進数の基数
    private const int DECIMAL_BASE = 10;
    //値をカウントできる限界値は99(2桁のため)
    private const int MAX_WING_COUNT = 99;
    //値のデフォルト値
    private const int DEFAULT_WING_COUNT = 0;

    //0~9に数字のスプライトを入れる
    [SerializeField] private Sprite[] _sprites = default;
    //左から１番目の数字のImage
    [SerializeField] private Image _firstNumber = default;
    //左から2番目の数字のImage
    [SerializeField] private Image _secondNumber = default;
    //出てきたり消えたりするアニメーションを再生させるために必要
    [SerializeField] private Animator _animator = default;
    //羽取得効果音を再生するためにAudioManagerを取得
    [SerializeField] private AudioManager _audioManager = default;
    //デフォルトのピッチ値
    [SerializeField] private float _defaultPitch = 0.5f;
    //羽取得時どれくらいピッチが上がっていくか
    [SerializeField] private float _addPitch = 0.1f;
    //ピッチのMAX値
    [SerializeField] private float _maxPitch = 2.0f;
    //羽の数をカウントするための数値
    private int _wingCount = DEFAULT_WING_COUNT;

    /// <summary>
    /// 羽を取った時に実行する
    /// 引数には取った羽によるゲージの回復量を入れる
    /// (普通の羽と金の羽が同じ+1と表現されるのはおかしいと考えたためだが、
    /// 　普通の羽を取って+20とかになるのもなんかなあと思うので要相談かも）
    /// </summary>
    /// <param name="num"></param>
    public void GetWing(int num)
    {
        _wingCount = Mathf.Min(_wingCount + num, MAX_WING_COUNT);
        //１桁ならfirstに数字を出し、secondは非表示にするため、ここで分ける
        if (_wingCount < DECIMAL_BASE)
        {
            _secondNumber.enabled = false;
            _firstNumber.sprite = _sprites[_wingCount];
        }
        else
        {
            _secondNumber.enabled = true;
            //intだから切り下げられる firstの値を取り出す 2桁目
            int _firstWingCount = (int)(_wingCount / DECIMAL_BASE);
            //secondの値を取り出す 1桁目
            int _secondWingCount = _wingCount % DECIMAL_BASE;

            _firstNumber.sprite = _sprites[_firstWingCount];
            _secondNumber.sprite = _sprites[_secondWingCount];
        }
        _animator.SetTrigger("GetWing");
    }
    /// <summary>
    /// 羽のカウントを０に戻す処理
    /// アニメーションイベントから実行する
    /// (再生しきった時に実行する)
    /// </summary>
    public void ResetWingCount()
    {
        _wingCount = DEFAULT_WING_COUNT;
    }
}
