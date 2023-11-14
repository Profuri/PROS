using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSceneScreen : UGUIComponent
{
    [SerializeField] private UGUIMenuSystem _menuSystem;
    
    public override void Init()
    {
        // Do Nothing
    }

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
    
    public void OnlineMenuCallBack()
    {
        
    }

    public void OptionsMenuCallBack()
    {
        
    }

    public void QuitMenuCallBack()
    {
        Application.Quit();
    }
    
    #endregion
}
