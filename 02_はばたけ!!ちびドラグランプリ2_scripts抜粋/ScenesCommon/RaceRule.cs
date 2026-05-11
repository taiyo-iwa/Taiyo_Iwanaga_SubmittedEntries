using UnityEngine;

public enum RaceRuleState
{
    Tutorial,
    Race,
    TimeAttack,
}

public class RaceRule : MonoBehaviour
{
    [SerializeField] private RaceRuleState _selectedRule = default;
    [SerializeField] private int _maxLap = 3;

    public RaceRuleState SelectedRule { get { return _selectedRule; } }
    public int MaxLap { get { return _maxLap; } }
}
