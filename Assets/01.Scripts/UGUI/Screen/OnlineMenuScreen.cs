using UnityEngine;

public class OnlineMenuScreen : UGUIComponent
{
    [SerializeField] private UGUIMenuSystem _menuSystem;

    public override void GenerateUI(Transform parent, EGenerateOption options)
    {
        base.GenerateUI(parent, options);
        _menuSystem.SetUp(this);
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
        UIManager.Instance.GenerateUGUI("ModeSelectScreen", EGenerateOption.CLEAR_PANEL | EGenerateOption.STACKING | EGenerateOption.RESETING_POS);
    }

    public void QuickMatchCallBack()
    {
        
    }

    public void RoomListCallBack()
    {
        UIManager.Instance.GenerateUGUI("RoomListScreen", EGenerateOption.CLEAR_PANEL | EGenerateOption.STACKING | EGenerateOption.RESETING_POS);
    }

    public void BackCallBack()
    {
        UIManager.Instance.ReturnUGUI();
    }
    
    #endregion
}