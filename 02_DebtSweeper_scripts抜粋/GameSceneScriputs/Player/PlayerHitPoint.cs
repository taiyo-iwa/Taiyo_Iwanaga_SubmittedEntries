using UnityEngine;

public class PlayerHitPoint : MonoBehaviour
{
    private const int PLAYER_MAX_HP = 100;

    PlayerDeath _playerDeath;
    PlayerHitPointText _playerHitPointText;

    private int _currentHP = PLAYER_MAX_HP;
    private bool _isAlive = true;

    public void PlauerHitPointStart(PlayerDeath playerDeath, PlayerHitPointText playerHitPointText)
    {
        _playerDeath = playerDeath;
        _playerHitPointText = playerHitPointText;
    }

    public void PlayerDamage(int damageValue)
    {
        _currentHP -= damageValue; 
        if (_currentHP <= 0 && _isAlive)
        {
            _isAlive = false;
            _currentHP = 0;
            _playerDeath.DeathPlayer();
            _playerHitPointText.CurrentHPTextController(_currentHP);
        }
        _playerHitPointText.DamegePanelOpen();
        _playerHitPointText.CurrentHPTextController(_currentHP);
    }

    public void PlayerHeal(int healValue)
    {
        _currentHP += healValue;    
        if (_currentHP >= PLAYER_MAX_HP)
        {
            _currentHP = PLAYER_MAX_HP;
        }
        _playerHitPointText.CurrentHPTextController(_currentHP);
    }

    private void PlayerMaxHPUpdate()
    {
        _playerHitPointText.MaxHPTextController(PLAYER_MAX_HP);
    }
}
