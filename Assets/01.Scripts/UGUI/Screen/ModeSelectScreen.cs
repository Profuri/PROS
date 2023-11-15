using UnityEngine;

public class ModeSelectScreen : UGUIComponent
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
        // Do Nothing
    }

    #region CallBacks

    public void SurviveCallBack()
    {
        
    }

    public void DeathMatchCallBack()
    {
        
    }

    public void OccupationCallBack()
    {
        
    }

    public void BackCallBack()
    {
        UIManager.Instance.ReturnUGUI();
    }
    
    #endregion
}