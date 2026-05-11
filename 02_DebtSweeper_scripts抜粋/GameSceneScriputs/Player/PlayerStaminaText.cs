using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaText : MonoBehaviour
{
    [SerializeField] private Text _currentStaminaText = default;
    [SerializeField] private Text _maxStaminaText = default;

    public void CurrentStaminaTextController(int currentStamina)
    {
        _currentStaminaText.text = currentStamina.ToString();
    }

    public void MaxStaminaTextController(int maxStamina)
    {
        _maxStaminaText.text = "/" + maxStamina.ToString();
    }
}
