using UnityEngine;
using UnityEngine.UI;

public class PlayerChargeDashView : MonoBehaviour
{    
    [SerializeField] private Scrollbar _dashChargeBar = default;
    [SerializeField] private Image[] _dashChargeBerImage = default;

    public void DashChargeBerController(float time)
    {
        _dashChargeBar.size = time;

        if(time <= 0.0f)
        {
            foreach (Image image in _dashChargeBerImage)
            {
                //Alpha‚ð•ÏX
                Color color = image.color;
                color.a = 0.0f;
                image.color = color;
            }
        }
        else
        {
            foreach (Image image in _dashChargeBerImage)
            {
                //Alpha‚ð•ÏX
                Color color = image.color;
                color.a = 1.0f;
                image.color = color;
            }
        }
    }
}
