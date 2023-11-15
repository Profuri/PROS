using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCard : UGUIComponent
{
    private Image _playerHeadImage;
    private TextMeshProUGUI _playerNickNameTMP;

    public override void GenerateUI(Transform parent, EGenerateOption options)
    {
        base.GenerateUI(parent, options);

        _playerHeadImage = transform.Find("PlayerIcon/PlayerHeadImage").GetComponent<Image>();
        _playerNickNameTMP = transform.Find("NickNameText").GetComponent<TextMeshProUGUI>();
    }

    public override void UpdateUI()
    {
        // Do Nothing
    }

    public void SetColor(Color color)
    {
        _playerHeadImage.color = color;
    }

    public void SetNickName(string nickName)
    {
        _playerNickNameTMP.text = nickName;
    }
}