using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RoomCard : UGUIComponent
{
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private TextMeshProUGUI _modeTMP;
    [SerializeField] private TextMeshProUGUI _playerCntTMP;

    [SerializeField] private Button _button;

    public override void RemoveUI()
    {
        base.RemoveUI();
        _button.onClick.RemoveAllListeners();
    }

    public override void UpdateUI()
    {
        // Do Nothing    
    }

    public void SetTitle(string title)
    {
        _titleTMP.text = title;
    }

    public void SetMode(EStageMode mode)
    {
        Debug.Log(mode);
        _modeTMP.text = mode.ToString();
    }

    public void SetPlayerCnt(int cur, int max)
    {
        _playerCntTMP.text = $"{cur} / {max}";
    }

    public void SetButtonCallBack(UnityAction CallBack)
    {
        _button.onClick.AddListener(CallBack);
    }
}