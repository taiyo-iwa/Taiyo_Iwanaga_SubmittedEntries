using UnityEngine;

[CreateAssetMenu(fileName = "ChargeCurve", menuName = "Scriptable Objects/ChargeCurve")]
public class ChargeCurve : ScriptableObject
{
	// üŒ`
	public AnimationCurve linear = AnimationCurve.Linear(
		timeStart: 0f,
		valueStart: 0f,
		timeEnd: 1f,
		valueEnd: 1f
	);

	public float GetValue(float time)
	{
		return linear.Evaluate(time);
	}
}
