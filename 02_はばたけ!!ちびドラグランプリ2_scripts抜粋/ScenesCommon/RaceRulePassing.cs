using UnityEngine;

public class RaceRulePassing : MonoBehaviour
{
    [SerializeField] private RacerRuleSO _racerRuleSO = default;
    [SerializeField] private int _racerRuleIndex = 0;

    public void OnRaceRulePassing()
    {
        _racerRuleSO.SetResult(_racerRuleIndex);
    }
}
