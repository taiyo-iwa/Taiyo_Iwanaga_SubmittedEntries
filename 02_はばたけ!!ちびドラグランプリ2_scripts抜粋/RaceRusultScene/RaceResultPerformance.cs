using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RaceResultPerformance : MonoBehaviour
{
    private const string SPEED = "Speed";
    private const string PERFORMANCE = "Performance";
    private const float DRAGON_SPEED = 4.0f;

    [SerializeField] private RacerNameSO _raceName = default;
    [SerializeField] private Animator _resultAnimation = default;
    [SerializeField] private AnimationEventDelivery[] deliverys;
    [SerializeField] private GameObject[] characterModels = default;
    [SerializeField] private Animator[] characterAnimators = default;
    [SerializeField] private SpriteRenderer _backGround = default;
    [SerializeField] private Image _rankImage = default;
    [SerializeField] private Sprite[] backGroundSprites = default;
    [SerializeField] private Sprite[] rankImageSprite = default;
    [SerializeField] private Text[] characterNameTexts = default;
    [SerializeField] private Text[] topNameTexts = default;

    private string[] characterNames = default;

    public void ResultPerformance(List<int> racerIdList, int selectRacer)
    {
        foreach (AnimationEventDelivery delivery in deliverys)
        {
            delivery.OnFinishPerformance
            .Subscribe(_ =>
            {
                ResultUIDisplay();
            })
            .AddTo(this);
        }

        CharacterAnimation(selectRacer);
        ChangeRankName(racerIdList, selectRacer);
    }

    private void ResultUIDisplay()
    {
        _resultAnimation.SetTrigger(PERFORMANCE);
    }

    private void CharacterAnimation(int selectRacer)
    {
        //モデルを消してリセットする
        foreach (GameObject character in characterModels)
        {
            character.SetActive(false);
        }

        //プレイヤーのアニメーションを発火する
        characterModels[selectRacer].SetActive(true);
        characterAnimators[selectRacer].SetFloat(SPEED, DRAGON_SPEED);
        characterAnimators[selectRacer].SetTrigger(PERFORMANCE);
    }

    private void ChangeRankName(List<int> racerIdList, int selectRacer)
    {
        //キャラクターの名前を取得
        characterNames = new string[_raceName.RacerNameList.Count];
        for (int i = 0; i < characterNames.Length; i++)
        {
            characterNames[i] = _raceName.RacerNameList[i];
        }

        //順位のTextを表示させる
        for (int i = 0; i < racerIdList.Count; i++)
        {
            characterNameTexts[i].text = characterNames[racerIdList[i]];
        }
        //トップの名前を表示する
        foreach (Text text in topNameTexts)
        {
            text.text = characterNames[selectRacer];
        }
    }

    public void ChangeRankUI(int playerRank)
    {
        //順位は１～５のため配列の添字に合わせる
        _backGround.sprite = backGroundSprites[playerRank - 1];

        _rankImage.sprite = rankImageSprite[playerRank - 1];
    }
}
