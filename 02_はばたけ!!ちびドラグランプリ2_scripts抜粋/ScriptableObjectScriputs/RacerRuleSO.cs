using UnityEngine;

[CreateAssetMenu(fileName = "RacerRuleSO", menuName = "Scriptable Objects/RacerRuleSO")]
public class RacerRuleSO : ScriptableObject
{
	public int RacerRuleIndex;

	public int SetResult(int ruleIndex)
	{
		return RacerRuleIndex = ruleIndex;
	}
}
