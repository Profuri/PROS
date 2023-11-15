using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSceneScreen : UGUIComponent
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

    #region CallBacks
    
    public void OnlineCallBack()
    {
        UIManager.Instance.GenerateUGUI("OnlineMenuScreen", EGenerateOption.CLEAR_PANEL | EGenerateOption.STACKING | EGenerateOption.RESETING_POS);
    }

    public void OptionsCallBack()
    {
        Debug.Log("option");
    }

    public void QuitCallBack()
    {
        Application.Quit();
    }
    
    #endregion
}
