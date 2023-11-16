using UnityEngine;

public class RoomListScreen : UGUIComponent
{
    [SerializeField] private InputSO _inputSO;
    
    private List<

    public override void GenerateUI(Transform parent, EGenerateOption options)
    {
        base.GenerateUI(parent, options);

        _inputSO.OnBackPress += BackCallBack;
        _inputSO.OnRefreshPress += RefreshCallBack;
    }

    public override void RemoveUI()
    {
        base.RemoveUI();
        
        _inputSO.OnBackPress -= BackCallBack;
        _inputSO.OnRefreshPress -= RefreshCallBack;
    }

    public override void UpdateUI()
    {
        // Do Nothing
    }
    
    #region CallBacks

    public void EnterRoomCallBack()
    {
        
    }

    private void RefreshCallBack()
    {
        
    }

    private void BackCallBack()
    {
        UIManager.Instance.ReturnUGUI();
    }
    
    #endregion
}