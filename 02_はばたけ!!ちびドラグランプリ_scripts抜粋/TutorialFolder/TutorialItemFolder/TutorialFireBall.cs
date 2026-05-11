using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialFireBall : MonoBehaviour
{
    [SerializeField] private ParticleSystem _fireballParticle = default;
    [SerializeField] private ParticleSystem _explosionParticle = default;

    private TutorialPlayerWarning _playerWarning = default;
    //追いかける対象のドラゴンを入れる
    private Transform _targetDragon = default;
    //90度以上離れていたら追従しない　という処理を作るための定数
    private float TARGET_ANGLE_RANGE = 60;
    //追尾を始めるまでの時間
    private float _trackingTimer = 0.3f;
    //最初ははじめの向きに沿って動くため、始めの向きを記録しておく
    private Vector3 _startForward = default;
    //空を飛んでいたら追従しないようにするため、PlayerMoveを取得する
    private TutorialPlayerMove _playerMove = default;
    //炎を飛ばしたドラゴンをとる　自分自身に当たらないために
    private Transform _makerDragon = default;
    //当たったかどうか
    private bool hitting = false;

    //追従判定
    public bool _isMoveToTarget { get; private set; } = false;


    private float _speed = 40;

    //このオブジェクトを生成したときにこれを呼び出す
    //追いかける対象であるドラゴンと、向きを設定する
    public void SetUp(Transform maker, Transform[] dragons)
    {

        _makerDragon = maker;
        _startForward = transform.forward;
        //一番近いドラゴンを入れる
        float nearDragonSqrDistance = default;
        Transform nearDragon = default;
        for (int i = 0; i < dragons.Length; i++)
        {
            if (dragons[i] == null)
            {
                Debug.LogError($"dragons[{i}] が null です！");
                continue;
            }

            TutorialPlayerMove playerMove = dragons[i].GetComponent<TutorialPlayerMove>();
            if (dragons[i] == _makerDragon)
                continue;
            //正面と追いかける対象のドラゴンとの角度差を求める
            float angleDifference =
                Vector3.Angle(transform.forward, (dragons[i].position - transform.position));

            //ドラゴンが空を飛んでいないか調べる　飛んでいた場合は追従しない
            //TutorialPlayerMove playerMove = dragons[i].gameObject.GetComponent<TutorialPlayerMove>();

            //nearDragonが存在しない場合は入れる
            //存在する場合は距離を比較して短ければ入れる
            float dragonSqrDistance = Vector3.SqrMagnitude(dragons[i].position - transform.position);
            if (!nearDragon || (nearDragon && dragonSqrDistance < nearDragonSqrDistance))
            {
                _isMoveToTarget = true;
                nearDragon = dragons[i];
                _playerMove = playerMove;
                nearDragonSqrDistance = dragonSqrDistance;
            }
        }
        //ターゲットの設定
        _targetDragon = nearDragon;
        if (_targetDragon)
        {
            _playerWarning = _targetDragon.GetComponent<TutorialPlayerWarning>();
            _playerWarning.StartWarningSprite(transform, _targetDragon, _isMoveToTarget);
        }
        Update();
    }



    private void Update()
    {
        if (hitting)
            return;
        if (_isMoveToTarget)
        {
            _trackingTimer -= Time.deltaTime;

            //このフレームの最終的なポジション
            Vector3 endVector = default;
            if (_trackingTimer > 0)
            {
                Vector3 firstVector = new Vector3(_startForward.x, 0, _startForward.z) * _speed * Time.deltaTime;
                Vector3 vectorToDragon = (_targetDragon.position - transform.position);
                Vector3 secondVector = new Vector3(vectorToDragon.x, 0, vectorToDragon.z).normalized * _speed * Time.deltaTime;
                endVector = transform.position + Vector3.Lerp(firstVector, secondVector, 1 - Mathf.Clamp01(_trackingTimer / 0.2f));
            }
            else
            {
                Vector3 vectorToDragon = (_targetDragon.position - transform.position);
                endVector = transform.position + new Vector3(vectorToDragon.x, 0, vectorToDragon.z).normalized * _speed * Time.deltaTime;

                //ｙを無視して指定以上近づいていた場合、
                //プレイヤーが空を飛んでいた場合は追従を切る
                float dragonDistance = Vector3.SqrMagnitude(
                    new Vector3(_targetDragon.position.x - transform.position.x,
                    0,
                    _targetDragon.position.z - transform.position.z));
                _isMoveToTarget = false;
                //_playerWarning.RemoveFireball(transform);
                //_playerWarning.OffWarningSprite(_isMoveToTarget);
            }
            transform.LookAt(endVector);
            //transform.rotation = Quaternion.Euler
            //    (new Vector3(0, transform.eulerAngles.y, 0));
            transform.position = endVector;

        }
        else
        {
            transform.position += new Vector3(transform.forward.x, 0, transform.forward.z) * _speed * Time.deltaTime;
        }

        //飛ばす場所　Fireballの位置より少し上
        Vector3 origin = transform.position + Vector3.up;
        //飛ばす長さ 
        float rayLength = 1.52f;
        //飛ばす方向　
        Vector3 dir = Vector3.down * rayLength;

        Debug.DrawRay(origin, dir, Color.red);
        RaycastHit hit;

        if (Physics.Raycast(origin, dir, out hit, rayLength, 1 << 8))
        {
            transform.position = hit.point + Vector3.up * 0.5f;
        }
        else
        {
            transform.position += Vector3.down * 5 * Time.deltaTime;
        }
        //球が当たったかどうかの判定をとる
        Collider[] hitColliders = Physics.OverlapSphere(
        transform.position,
        0.25f,
        (1 << 8 | 1 << 7 | 1 << 6)
        );
        if (hitColliders.Length > 0)
        {
            if (hitColliders.Length == 1 && hitColliders[0].transform == _makerDragon)
                return;
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.tag == "Player")
                {
                    //ここにプレイヤーにダメージ（？）を与える処理を付ける（マージした時）
                    print($"{hitCollider.gameObject.name}はダメージを受けた");
                    TutorialPlayerHitAction hitAction = hitCollider.GetComponent<TutorialPlayerHitAction>();
                    hitAction.FlameHit();
                }
                if (_playerWarning)
                    _playerWarning.RemoveFireball(transform);
            }
            hitting = true;
            _fireballParticle.Stop();
            _explosionParticle.Play();
            Destroy(this.gameObject, 5);
        }
    }
}
