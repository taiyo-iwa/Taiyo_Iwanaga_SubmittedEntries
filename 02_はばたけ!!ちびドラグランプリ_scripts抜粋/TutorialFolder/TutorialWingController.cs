using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialWingController : MonoBehaviour
{
    /* 翼のimage */
    [SerializeField] Image[] wing;
    [SerializeField] Image[] bothwings;

    /* ゲージスクリプト */
    [SerializeField] TutorialFeatherGaugeController featherGaugeController;

    private float _featherGauge;    /* 現在のゲージ量 */

    private bool isFlying;         /* 飛んでいるかどうか */

    private float _canFeatherGauge; /* 飛べるゲージ量 */

    private void Start()
    {
        /* 初期設定　透明化にならないように */
        if (wing[0] != null)
        {
            var wingColor = wing[0].color;
            wingColor.a = 1f;
            wing[0].color = wingColor;
        }
        /* 初期設定　透明化 */
        if (bothwings[0] != null)
        {
            var bothwingsColor = bothwings[0].color;
            bothwingsColor.a = 0f;
            bothwings[0].color = bothwingsColor;
        }
    }

    private void Update()
    {
        /* 現在の状態を取得 */
        //isFlying = featherGaugeController.IsFlying;

        /* 飛べないゲージ量 */

        //ここ勝手に書き換えちゃったごめん
        if (wing[0] != null && bothwings[0] != null)
        {
            if (!featherGaugeController.CanFly)
            {
                /* 片翼を有効化 */
                var wingColor = wing[0].color;
                wingColor.a = 1f;
                wing[0].color = wingColor;

                /* 両翼を透明化 */
                var bothwingsColor = bothwings[0].color;
                bothwingsColor.a = 0f;
                bothwings[0].color = bothwingsColor;
            }
            /* 飛べるゲージ量 */
            else
            {
                /* 片翼を透明化 */
                var wingColor = wing[0].color;
                wingColor.a = 0f;
                wing[0].color = wingColor;

                /* 両翼を有効化 */
                var bothwingsColor = bothwings[0].color;
                bothwingsColor.a = 1f;
                bothwings[0].color = bothwingsColor;
            }
        }
    }
}
