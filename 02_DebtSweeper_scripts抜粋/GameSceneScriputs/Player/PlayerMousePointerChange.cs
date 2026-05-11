using UnityEngine;
using UnityEngine.UI;

public class PlayerMousePointerChange : MonoBehaviour
{
    [SerializeField] Image _pointerImage = default;
    [SerializeField] Sprite _normalPointer = default;
    [SerializeField] Sprite _discoveryPointer = default;
    [SerializeField] Sprite _grabsPointer = default;

    public void ChangePointerSprite(bool canGrabObject)
    {
        if (canGrabObject)
        {
            _pointerImage.sprite = _discoveryPointer;
            _pointerImage.color = Color.yellow;
        }
        else
        {
            _pointerImage.sprite = _normalPointer;
            _pointerImage.color = Color.white;
        }
    }

    public void GrabbedPointerSprite()
    {
        _pointerImage.sprite = _grabsPointer;
        _pointerImage.color = Color.white;
    }
}
