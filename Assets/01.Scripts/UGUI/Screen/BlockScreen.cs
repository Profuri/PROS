using UnityEngine;

public class BlockScreen : UGUIComponent
{
    public override void GenerateUI(Transform parent, EGenerateOption options)
    {
        base.GenerateUI(parent, options);
        NetworkManager.Instance.OnConnectedToMasterEvent += UIManager.Instance.RemoveTopUGUI;
    }

    public override void RemoveUI()
    {
        base.RemoveUI();
        UIManager.Instance.GenerateUGUI("NickNameInputScreen", EGenerateOption.BLUR | EGenerateOption.STACKING | EGenerateOption.RESETING_POS);
        NetworkManager.Instance.OnConnectedToMasterEvent -= UIManager.Instance.RemoveTopUGUI;
    }

    public override void UpdateUI()
    {
        // Do Nothing
    }
}