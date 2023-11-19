using UnityEngine;

public class BlockScreen : UGUIComponent
{
    public override void RemoveUI()
    {
        base.RemoveUI();
        UIManager.Instance.GenerateUGUI("NickNameInputScreen", EGenerateOption.BLUR | EGenerateOption.STACKING | EGenerateOption.RESETING_POS);
    }

    public override void UpdateUI()
    {
        // Do Nothing
    }
}