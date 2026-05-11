using UnityEngine;
using UnityEngine.UI;

public class RaceRankingView : MonoBehaviour
{
    [SerializeField] private RaceRule _raceRule = default;
    [SerializeField] private RacerNameSO _raceName;
    [SerializeField] private Image _characterImage;
    [SerializeField] private Sprite[] characterIconSprites;
    [SerializeField] private Text _rankText = default;
    [SerializeField] private Text _nameText = default;
    [SerializeField] private Text _lapText = default;

    private string[] characterNames = default;

    public void Awake()
    {
        //キャラクターの名前を取得
        characterNames = new string[_raceName.RacerNameList.Count];

        for(int i = 0; i < characterNames.Length; i++)
        {
            characterNames[i] = _raceName.RacerNameList[i];
        }
    }

    public void SetData(int rank, int racerId, int lap)
    {
        _rankText.text = rank.ToString();
        string racerName = "";
        Sprite iconSprite = null;
        switch (racerId)
        {
            case 0:
                racerName = characterNames[racerId];
                iconSprite = characterIconSprites[racerId];
                break;
            case 1:
                racerName = characterNames[racerId];
                iconSprite = characterIconSprites[racerId];
                break;
            case 2:
                racerName = characterNames[racerId];
                iconSprite = characterIconSprites[racerId];
                break;
            case 3:
                racerName = characterNames[racerId];
                iconSprite = characterIconSprites[racerId];
                break;
            case 4:
                racerName = characterNames[racerId];
                iconSprite = characterIconSprites[racerId];
                break;
        }
        _nameText.text = racerName;
        _characterImage.sprite = iconSprite;
        if(lap > _raceRule.MaxLap)
        {
            return;
        }
        _lapText.text = lap.ToString() + " / " + _raceRule.MaxLap.ToString();
    }
}
