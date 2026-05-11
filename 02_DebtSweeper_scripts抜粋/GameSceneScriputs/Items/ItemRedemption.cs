using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemRedemption : MonoBehaviour
{
    private const float CASHING_QUOTA = 400.0f;
    private const float WAIT_SECONDS = 2f;

    [SerializeField] private RedemptionAnimationController _redemptionAnimation = default;
    [SerializeField] private TextMeshPro _textMeshProUGUI = default;
    [SerializeField] private Text _currentValueText = default;

    private PickupableItem _pickupableItem = default;
    private float _redemptionItemPrice = 0.0f;

    private void OnTriggerEnter(Collider other)
    { 
        _pickupableItem = other.GetComponent<PickupableItem>();
        if(_pickupableItem != null)
        {
            EnterRedemptionItem();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _pickupableItem = other.GetComponent<PickupableItem>();
        if (_pickupableItem != null)
        {
            ExitRedemptionItem();
        }
    }

    private async void EnterRedemptionItem()
    {
        if (_pickupableItem.IsRedemptionItem())
        {
            float price = _pickupableItem.Redemption();
            _redemptionItemPrice += price;
            _textMeshProUGUI.text = _redemptionItemPrice.ToString();
            _currentValueText.text = _redemptionItemPrice.ToString();
            if (_redemptionItemPrice >= CASHING_QUOTA)
            {
                await _redemptionAnimation.StartCashExchange(WAIT_SECONDS, _redemptionItemPrice, CASHING_QUOTA);    
            }
        }     
    }

    private void ExitRedemptionItem()
    {
        if (_pickupableItem.IsRedemptionItem())
        {
            float price = _pickupableItem.Redemption();
            _redemptionItemPrice -= price;
            _textMeshProUGUI.text = _redemptionItemPrice.ToString();
            _currentValueText.text = _redemptionItemPrice.ToString();
            if (_redemptionItemPrice < CASHING_QUOTA)
            {
                _redemptionAnimation.CancelWaiting();
            }
        }
    }
}
