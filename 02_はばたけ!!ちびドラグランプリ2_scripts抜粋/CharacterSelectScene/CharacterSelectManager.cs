using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class CharacterSelectManager : MonoBehaviour
{
    [SerializeField] private CharacterUIController _characterUIController;
    [SerializeField] private CharacterOperationController _characterOperationController;
    [SerializeField] private GameObject[] _characterButton = default;
    
    private GameObject _previousSelectedButton = default;

    private void Update()
    {
        GameObject selectedButton = EventSystem.current.currentSelectedGameObject;

        if(selectedButton == _previousSelectedButton)
        {
            return;
        }
        int index = Array.IndexOf(_characterButton, selectedButton);
        if (index >= 0)
        {
            _characterUIController.SelectCharacter(index);
            _characterOperationController.SelectingButton(index);
        }
        _previousSelectedButton = selectedButton;
    }
}
