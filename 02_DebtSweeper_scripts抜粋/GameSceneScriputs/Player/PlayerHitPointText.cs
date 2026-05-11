using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class PlayerHitPointText : MonoBehaviour
{
    [SerializeField] private Text _currentHPText = default;
    [SerializeField] private Text _maxHPText = default;
    [SerializeField] private Image _damegePanel = default;

    public void CurrentHPTextController(int currentHP)
    {
        _currentHPText.text = currentHP.ToString();
    }

    public void MaxHPTextController(int maxHP)
    {
        _maxHPText.text = maxHP.ToString();
    }

    public async void DamegePanelOpen()
    {
        await DamegePanelOpenWait();
    }

    private async UniTask DamegePanelOpenWait()
    {
        _damegePanel.enabled = true;
        await UniTask.WaitForSeconds(0.5f);
        _damegePanel.enabled = false;
    }
}
