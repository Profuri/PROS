using TMPro;

public class RoomCard : UGUIComponent
{
    private TextMeshProUGUI _titleTMP;
    private TextMeshProUGUI _modeTMP;
    private TextMeshProUGUI _playerCntTMP;
    
    public override void UpdateUI()
    {
        
    }

    public void SetTitle(string title)
    {
        _titleTMP.text = title;
    }

    public void SetMode(EStageMode mode)
    {
        _modeTMP.text = mode.ToString();
    }
}