using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPlayerWarning : MonoBehaviour
{
    private const float _maxFlamePositionX = 390;

    [SerializeField] Transform _playerDragon = default;

    //他スクリプトから、１ｐ、２ｐにそれぞれ動かすフレームを割り当てる　Nullの場合はAIと判断する
    public GameObject _flameWarning { get; set; } = default;

    [SerializeField] private TutorialSoundController _soundController;
    private HashSet<Transform> _fireBallTransforms = new HashSet<Transform>();
    private RectTransform _flameWarnigTransform = default;
    private Image _flameWarningImage = default;
    //UIを見えなくする処理を実行するため、発見しているtrueにしておく
    private bool _isSearch = true;


    private void Start()
    {
        if (_flameWarning == null)
            return;
        _flameWarnigTransform = _flameWarning.GetComponent<RectTransform>();
        _flameWarningImage = _flameWarning.GetComponent<Image>();
    }
    void Update()
    {
        //Nullの場合はAI判定のため実行しない
        if (!_flameWarning)
            return;
        TransformWarningSprite();
        if (_isSearch)
        {
            FlashingFlameWarningImage();
        }
    }

    public void StartWarningSprite(Transform fireBallPosition, Transform targetDragon, bool following)
    {
        //Nullの場合はAI判定のため実行しない
        if (!_flameWarning)
            return;
        _fireBallTransforms.Add(fireBallPosition);
        if (_playerDragon == targetDragon && following && _fireBallTransforms.Count == 1)
        {
            _soundController.PlayWarningSound();
            //print(_playerDragon.name + "が狙われている");
        }
    }

    public void RemoveFireball(Transform fireball)
    {
        _fireBallTransforms.Remove(fireball);
        if (_fireBallTransforms.Count == 0)
        {
            _soundController.StopWarningSound();
        }
    }

    private void TransformWarningSprite()
    {
        //一番ドラゴンから近いFireballを指定して、UIを動かす
        Transform targetFireballTransform =
            SearchNearFireBall();
        //発見状態に変化がある場合は、UIの表示非表示を切り替える
        if (_isSearch != targetFireballTransform)
        {
            _isSearch = targetFireballTransform;
            _flameWarning.SetActive(_isSearch);
        }
        //追尾しているfireballがない場合はUIを動かす処理を実行しない
        if (!targetFireballTransform)
        {
            return;
        }
        //プレイヤーとFireBallとの距離
        Vector3 dir = targetFireballTransform.position - _playerDragon.position;
        //プレイヤーに対してFireBallが向いている角度
        float angle = Vector3.Angle(-(_playerDragon.forward), dir);
        //プレイヤーが敵の左右どちらにいるか
        Vector3 cross = Vector3.Cross(_playerDragon.forward, dir.normalized);
        float direction = cross.y;
        //左右どちらにいるかで、angleの正負を決める
        #region FireBallが左右どちらにいるか
        if (direction > 0)
        {
            //右にいる時
            angle *= 1;
        }
        else if (direction < 0)
        {
            //左にいる時
            angle *= -1;
        }
        else
        {
            //真ん中の時
            angle = 0;
        }
        #endregion

        //flameWarnigの移動範囲を大きくする
        angle *= 10;
        //flameWarnigのImageをCanvasの範囲内に収める
        if (angle < -(_maxFlamePositionX))
        {
            angle = -(_maxFlamePositionX);
        }
        else if (angle > _maxFlamePositionX)
        {
            angle = _maxFlamePositionX;
        }

        _flameWarnigTransform.anchoredPosition = new Vector2(angle, 0);

        //距離に応じて大きさを変える
        float dirSize = 0;
        dirSize = Mathf.Clamp(1 - dir.magnitude / 60, 0.5f, 1);

        _flameWarnigTransform.localScale = new Vector3(dirSize, dirSize, dirSize);
    }

    /// <summary>
    /// FlameWarningのImageを点滅させる
    /// </summary>
    private void FlashingFlameWarningImage()
    {
        Color color = _flameWarningImage.color;
        color.a = Mathf.Sin(Time.time * 5f) * 0.5f + 0.5f;
        _flameWarningImage.color = color;
    }
    /// <summary>
    /// このドラゴンに追尾してるFireBallから一番近いFireBallを返す
    /// 消滅しているFireBallをHashSetから削除する処理もついでに行う
    /// 一つも追尾してないならNullを返す
    /// </summary>
    private Transform SearchNearFireBall()
    {
        Transform nearFireball = default;
        float nearFireballDistanceSqr = default;
        if (_fireBallTransforms.Count == 0)
            return default;
        foreach (Transform fireball in _fireBallTransforms)
        {
            //近いファイヤーボールを見つけていない場合はそのまま更新、
            //見つけてる場合、比較して更新するかの判断をするため距離の計算をする
            float fireballDistanceSqr = Vector3.SqrMagnitude(transform.position - fireball.position);
            if (nearFireball == null || fireballDistanceSqr < nearFireballDistanceSqr)
            {
                nearFireball = fireball;
                nearFireballDistanceSqr = fireballDistanceSqr;
            }
        }
        //一番近いfireballを返す
        return nearFireball;
    }
}
