using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;
using System;
using UniRx;

public class CharacterOperationController : MonoBehaviour
{
    [SerializeField] private InputActionReference _cancelAction = default;
    [SerializeField] private RacerDataSO _racerDataSO = default;
    [SerializeField] private GameObject _parent = default;
    [SerializeField] private GameObject _nextButton = default;
    [SerializeField] private GameObject _panel = default;
    [SerializeField] private Button[] _characterButtons = default;
    [SerializeField] private Text _nextSelection = default;
    [SerializeField] private AudioSource _audioSource = default;
    [SerializeField] private AudioClip _pressAudio = default;
    [SerializeField] private AudioClip _cancelAudio = default;

    private Button[] buttons;
    private int _selectingCharacter = 0;
    private bool _selectedCharacter = false;
    private Subject<bool> _cancelStream = new Subject<bool>();

    private IObservable<bool> OnCancel
    {
        get { return _cancelStream; }
    }

    private void OnEnable()
    {
        _cancelAction.action.Enable();
        _cancelAction.action.started += _ => _cancelStream.OnNext(true);
        _cancelAction.action.canceled += _ => _cancelStream.OnNext(false);
    }

    private void OnDisable()
    {
        _cancelAction.action.Disable();
    }

    void Start()
    {
        OnCancel
        .Subscribe(isCancelButton => CancelButtonInput(isCancelButton))
        .AddTo(this);

        buttons = _parent.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => SelectCharacter());
        }
    }

    public void SelectingButton(int racerId)
    {
        _selectingCharacter = racerId;
    }

    private void SelectCharacter()
    {
        _selectedCharacter = true;

        List<RacerData> recers = new List<RacerData>();

        recers.Add(new RacerData
        {
            SelectRacerId = _selectingCharacter,
        });

        _racerDataSO.SetResult(recers);

        foreach (Button button in _characterButtons)
        {
            button.enabled = false;
        }
        _nextButton.SetActive(true);
        _panel.SetActive(true);
        _nextSelection.text = "Bボタンでキャンセル";
        EventSystem.current.SetSelectedGameObject(_nextButton);

        //音を鳴らす
        _audioSource.PlayOneShot(_pressAudio);
    }

    private void CancelButtonInput(bool isCancelButton)
    {
        if (!_selectedCharacter)
        {
            return;
        }
        if (isCancelButton)
        {
            //音を鳴らす
            _audioSource.PlayOneShot(_cancelAudio);

            _selectedCharacter = false;
            _nextButton.SetActive(false);
            _panel.SetActive(false);
            _nextSelection.text = "Aボタンで選択";

            foreach (Button button in _characterButtons)
            {
                button.enabled = true;
            }
            EventSystem.current.SetSelectedGameObject(_characterButtons[_selectingCharacter].gameObject);
        }
    }
}
