using UnityEngine;
using UnityEngine.UI;

public class TimerView : MonoBehaviour
{
    [SerializeField] private Text _totalTimeText = default;
    [SerializeField] private Text[] _lapTimeText = default;

    public void TotalTimeTextUpdate(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        int milliSeconds = Mathf.FloorToInt(time * 100 % 100);

        _totalTimeText.text = string.Format("{0:D1}'{1:D2}''{2:D2}", minutes, seconds, milliSeconds);
    }

    public void LapTimeTextUpdate(int lap, float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        int milliSeconds = Mathf.FloorToInt(time * 100 % 100);

        //現在のラップと配列の要素数を合わせるため-１する
        _lapTimeText[lap - 1].text = string.Format("{0:D1}'{1:D2}''{2:D2}", minutes, seconds, milliSeconds);
    }
}
