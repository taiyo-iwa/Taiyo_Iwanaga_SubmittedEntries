using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    PlayerStaminaText _playerStaminaText = default;

    public void PlayerStaminaStart(PlayerStaminaText playerStaminaText)
    {
        _playerStaminaText = playerStaminaText;
    }

    public void PlayerStaminaController(float currentStamina, int maxStamina)
    {
        int intCurrentStamina = (int)currentStamina;
        _playerStaminaText.CurrentStaminaTextController(intCurrentStamina);
        _playerStaminaText.MaxStaminaTextController(maxStamina);
    }
}
