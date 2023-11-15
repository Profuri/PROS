using UnityEngine;

public class NickNameInputScreen : UGUIComponent
{
    [SerializeField] private InputSO _inputSO;

    public override void GenerateUI(Transform parent, EGenerateOption options)
    {
        base.GenerateUI(parent, options);
    }

    public override void RemoveUI()
    {
        base.RemoveUI();
    }

    public override void UpdateUI()
    {
        // Do Nothing
    }
    
    #region CallBacks

    private void EnterCallBack()
    {
        
    }
    
    #endregion
}