using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSceneScreen : UGUIComponent
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
    
    public void OnlineCallBack()
    {
        UIManager.Instance.GenerateUGUI("OnlineMenuScreen", null, EGenerateOption.CLEAR_PANEL);
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
