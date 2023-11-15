using UnityEngine;

public class ModeSelectScreen : UGUIComponent
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
        // Do Nothing
    }

    private void CreateRoom(EStageMode mode)
    {
        Debug.Log(mode);
        NetworkManager.Instance.CreateRoom();
        UIManager.Instance.GenerateUGUI("WaitingRoomScreen", EGenerateOption.CLEAR_PANEL | EGenerateOption.STACKING | EGenerateOption.RESETING_POS);
    }

    #region CallBacks

    public void SurviveCallBack()
    {
        CreateRoom(EStageMode.SURVIVE);
    }

    public void DeathMatchCallBack()
    {
        CreateRoom(EStageMode.DEATHMATCH);
    }

    public void OccupationCallBack()
    {
        CreateRoom(EStageMode.OCCUPATION);
    }

    public void BackCallBack()
    {
        UIManager.Instance.ReturnUGUI();
    }
    
    #endregion
}