using TMPro;
using UnityEngine;
using WebSocketSharp;

public class NickNameInputScreen : UGUIComponent
{
    [SerializeField] private InputSO _inputSO;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private AudioClip _enterClip;

    public override void GenerateUI(Transform parent, EGenerateOption options)
    {
        base.GenerateUI(parent, options);
        
        _inputSO.OnEnterPress += EnterCallBack;
    }

    public override void RemoveUI()
    {
        base.RemoveUI();
        
        _inputSO.OnEnterPress -= EnterCallBack;
    }

    public override void UpdateUI()
    {
        // Do Nothing
    }
    
    #region CallBacks

    private void EnterCallBack()
    {
        var nickName = _inputField.text;

        if (nickName.IsNullOrEmpty())
        {
            Debug.LogError("NickName can't empty string");
            return;
        }

        if (nickName.Length > 8)
        {
            Debug.LogError("NickName lenght is min 8 char");
            return;
        }
        
        NetworkManager.Instance.LocalPlayer.NickName = nickName;
        NetworkManager.Instance.LocalPlayer.CustomProperties.Add("Score", 0);
        NetworkManager.Instance.LocalPlayer.CustomProperties.Add("R", Random.Range(0.5f, 1f));
        NetworkManager.Instance.LocalPlayer.CustomProperties.Add("G", Random.Range(0.5f, 1f));
        NetworkManager.Instance.LocalPlayer.CustomProperties.Add("B", Random.Range(0.5f, 1f));

        AudioManager.Instance.Play(_enterClip);
        AudioManager.Instance.PlayLobbyBGM();
        UIManager.Instance.RemoveTopUGUI();
    }
    
    #endregion
}