using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialWingItem : MonoBehaviour
{
    //respawnDelayTimerが0を下回ったら再度復活させて取れるようにする
    private const float RESPAWN_TRIGGER_THRESHOLD = 0f;
    //対象のドラゴンがプレイヤーである場合、羽取得アニメーションも再生させるためプレイヤーかどうか判別するためのもの
    [SerializeField] private TutorialGameManager _gameManager = default;
    [SerializeField] private GameObject[] _dragons = default;
    //羽取得アニメーションを再生させるためのスクリプトを取得
    [SerializeField] private TutorialGetWingUIController _getWingUIController = default;
    [SerializeField] private GetWingUIController _twoPlayerGetWingUIController = default;
    //羽取得時にパーティクルを再生させるために必要
    [SerializeField] private ParticleSystem _particleSystem = default;
    //羽のスプライトをカメラに向かせるため、それと取った時に非アクティブに変えるために取得
    [SerializeField] private SpriteRenderer[] _spriteRenderer = default;
    //パーティクルの見える状態、見えない状態を変えるために必要
    [SerializeField] private GameObject[] _particle = default;
    //カメラに向かせるためにカメラのTransformを取得
    [SerializeField] private Transform[] _cameraTransform = default;
    //羽取得アニメーションで値をいくら加算するかを設定
    [SerializeField] private int _healCountNumber = 1;
    //羽ゲージの溜まる量を設定
    private float _healGauge = 0.5f;
    //描画距離 (カメラに向かせる処理が大量にあると重くなるため、遠くのものは見せない）
    [SerializeField] private float _visibleDistance = 10f;
    //アイテムをとってから復活するまでの時間
    [SerializeField] private float _respawnDelay = 5f;
    //羽の連続取得　何回やるか
    [SerializeField] private int _getWingNum = 1;
    //羽の連続取得　何秒以内に取るか
    [SerializeField] private float _getWingTime = 0.5f;
    //羽の表示が小さくなり始める距離
    private float _wingSmallDistance = 500;
    //羽の通常サイズ(最大サイズ)
    private float _wingDefaultSize = 0.3f;
    //羽の最小サイズ
    private float _wingMinSize = 0.15f;
    //通常位置
    private Vector3 _wingDefaultPos = Vector3.zero;
    //最小位置
    private Vector3 _wingMinPos = new Vector3(0, -0.27f, 0);

    //アイテムをとってから復活するまでの時間を計測するための値
    private float _respawnDelayTimer = 0f;
    //表示するかどうか　状態が変わったときにのみ表示非表示の処理を動かすために必要
    private bool[] _isVisible = { true, true };
    //羽の連続取得　取った後それぞれ何秒待つかを決める
    private float _getOneWingTime = default;
    private void Start()
    {
        //sqrDistanceを使って処理をするため、2乗した値をキャッシュしておく(Distance処理が増えると重い)
        _visibleDistance = _visibleDistance * _visibleDistance;
        //１つあたりそれぞれ何秒待つかを計算
        _getOneWingTime = _getWingTime / _getWingNum;

        if (_gameManager._twoPlayerSelectDragon < 0)
        {
            _particle[1].SetActive(false);
            _spriteRenderer[1].enabled = false;
        }
    }
    //羽をカメラに向かせるための処理 
    /// <summary>
    /// 羽をカメラに向かせる、表示、非表示を切り替える処理
    /// 負荷軽減のため、FixedUpdateで処理を行う
    /// </summary>
    private void FixedUpdate()
    {
        float[] sqrDistanceToCamera = default;
        bool[] inVisibleDistance = default;
        if (_cameraTransform.Length >= 2)
        {
            //2p
            //距離を求める(平方根を求める前の距離)
            sqrDistanceToCamera = new float[]
                { (transform.position - _cameraTransform[0].position).sqrMagnitude,
                (transform.position - _cameraTransform[1].position).sqrMagnitude};
            //見える範囲内にいるかの判定
            inVisibleDistance = new bool[]
                { sqrDistanceToCamera[0] <= _visibleDistance,
                sqrDistanceToCamera[1] <= _visibleDistance};
        }
        else
        {
            //1p
            //距離を求める(平方根を求める前の距離)
            sqrDistanceToCamera = new float[]
                { (transform.position - _cameraTransform[0].position).sqrMagnitude};
            //見える範囲内にいるかの判定
            inVisibleDistance = new bool[]
                { sqrDistanceToCamera[0] <= _visibleDistance};
        }


        //アイテムが復活しているか(まだ取られてないか)の判定
        bool isNotGetting;
        //判定を取る　復活までの時間に到達していない場合は時間の計測処理も加える
        if (_respawnDelayTimer <= RESPAWN_TRIGGER_THRESHOLD)
        {
            isNotGetting = true;
        }
        else
        {
            _respawnDelayTimer -= Time.fixedDeltaTime;
            isNotGetting = false;
        }
        //見えるかどうかの判定が変わっていた場合は、変えるための処理を行う
        if (_isVisible[0] != (isNotGetting && inVisibleDistance[0]))
        {
            _isVisible[0] = !_isVisible[0];
            _spriteRenderer[0].enabled = _isVisible[0];
            _particle[0].SetActive(_isVisible[0]);
        }

        if (_cameraTransform.Length >= 2 && _isVisible[1] != (isNotGetting && inVisibleDistance[1]))
        {
            _isVisible[1] = !_isVisible[1];
            _spriteRenderer[1].enabled = _isVisible[1];
            _particle[1].SetActive(_isVisible[1]);
        }

        if (_isVisible[0])
            ChangeWingTransform(0, sqrDistanceToCamera[0], _cameraTransform[0].position);
        if (_cameraTransform.Length >= 2 && _isVisible[1])
            ChangeWingTransform(1, sqrDistanceToCamera[1], _cameraTransform[1].position);
    }

    public void ChangeWingTransform(int num, float distance, Vector3 lookAt)
    {
        _spriteRenderer[num].transform.LookAt(lookAt);
        float setSize = Mathf.Lerp(_wingMinSize, _wingDefaultSize,
            Mathf.Clamp01(distance / _wingSmallDistance));
        _spriteRenderer[num].transform.localScale = new Vector3(-setSize, setSize, setSize);
        _spriteRenderer[num].transform.localPosition = Vector3.Lerp(_wingMinPos, _wingDefaultPos, setSize);
    }

    private void OnTriggerEnter(Collider other)
    {
        //まだ復活していない場合は処理をしない
        if (_respawnDelayTimer > RESPAWN_TRIGGER_THRESHOLD)
            return;
        //フライトゲージを加算するために　かつ　ドラゴンかどうかを確かめるために取得
        PlayerFlightGauge flightGauge = other.gameObject.GetComponent<PlayerFlightGauge>();
        if (flightGauge != null)
        {
            _particleSystem.Play();
            flightGauge.Charge(_healGauge);
            _respawnDelayTimer = _respawnDelay;
            _isVisible[0] = false;
            if (_cameraTransform.Length >= 2)
                _isVisible[1] = false;
            _spriteRenderer[0].enabled = false;
            if (_cameraTransform.Length >= 2)
                _spriteRenderer[1].enabled = false;
            _particle[0].SetActive(false);
            if (_cameraTransform.Length >= 2)
                _particle[1].SetActive(false);
            if (_dragons[_gameManager._selectDragon] == other.gameObject)
            {
                StartCoroutine(GetWing());
            }
            if (_cameraTransform.Length >= 2 &&
                _dragons[_gameManager._twoPlayerSelectDragon] == other.gameObject)
            {
                StartCoroutine(TwoPlayerGetWing());
            }
        }
    }

    IEnumerator GetWing()
    {
        for (int i = 1; i <= _getWingNum; i++)
        {
            if (_getWingUIController)
                _getWingUIController.GetWing(_healCountNumber);
            yield return new WaitForSeconds(_getOneWingTime);
        }
    }

    IEnumerator TwoPlayerGetWing()
    {
        for (int i = 1; i <= _getWingNum; i++)
        {
            if (_twoPlayerGetWingUIController)
                _twoPlayerGetWingUIController.GetWing(_healCountNumber);
            yield return new WaitForSeconds(_getOneWingTime);
        }
    }
}
