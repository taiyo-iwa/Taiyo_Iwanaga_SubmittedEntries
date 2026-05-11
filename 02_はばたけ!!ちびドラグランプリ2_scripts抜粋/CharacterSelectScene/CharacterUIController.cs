using UnityEngine;
using UnityEngine.UI;

public class CharacterUIController : MonoBehaviour
{
    private const string SPEED = "Speed";
    private const float DRAGON_SPEED = 4.0f;

    [SerializeField] private RacerNameSO _raceName = default;
    [SerializeField] private GameObject[] _characterModel = default;
    [SerializeField] private Animator[] _characterAnimator = default;
    [SerializeField] private Text _characterNameText = default;
    [SerializeField] private Image[] _characterButtonImages = default;
    [SerializeField] private Sprite[] _normalCharacterSprites = default;
    [SerializeField] private Sprite[] _selectedCharacterSprites = default;
    [SerializeField] private AudioSource _audioSource = default;
    [SerializeField] private AudioClip _changeAudio = default;

    private string[] characterNames = default;

    private void Start()
    {
        //キャラクターの名前を取得
        characterNames = new string[_raceName.RacerNameList.Count];

        for (int i = 0; i < characterNames.Length; i++)
        {
            characterNames[i] = _raceName.RacerNameList[i];
        }
    }

    public void SelectCharacter(int index)
    {
        foreach (GameObject character in _characterModel)
        {
            character.SetActive(false);
        }
        _characterModel[index].SetActive(true);
        _characterAnimator[index].SetFloat(SPEED, DRAGON_SPEED);
        _characterNameText.text = characterNames[index];
        for (int i = 0; i < _characterButtonImages.Length; i++)
        {
            _characterButtonImages[i].sprite = _normalCharacterSprites[i];
        }
        _characterButtonImages[index].sprite = _selectedCharacterSprites[index];

        _audioSource.PlayOneShot(_changeAudio);
    }
}
