using UnityEngine;

public class OnlineMenuScreen : UGUIComponent
{
    [SerializeField] private UGUIMenuSystem _menuSystem;

    public override void GenerateUI(Transform parent, EGenerateOption options)
    {
        base.GenerateUI(parent, options);
        _menuSystem.SetUp();
    }

    public override void RemoveUI()
    {
        base.RemoveUI();
        _menuSystem.Release();
    }

    public override void UpdateUI()
    {
        
    }
    
    #region CallBacks

    public void CreateRoomCallBack()
    {
        UIManager.Instance.GenerateUGUI("ModeSelectScreen", null, EGenerateOption.CLEAR_PANEL);
    }

    public void QuickMatchCallBack()
    {
        
    }

    public void RoomListCallBack()
    {
        UIManager.Instance.GenerateUGUI("RoomListScreen", null, EGenerateOption.CLEAR_PANEL);
    }

    public void BackCallBack()
    {
        UIManager.Instance.ReturnUGUI();
    }
    
    #endregion
}