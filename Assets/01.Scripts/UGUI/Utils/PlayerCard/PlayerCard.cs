using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCard : UGUIComponent
{
    private Image _backGroundImage;
    private Image _playerHeadImage;
    private TextMeshProUGUI _playerNickNameTMP;
    private TextMeshProUGUI _readyTMP;

    private bool _ready;
    public bool Ready => _ready;

    [SerializeField] private Color _unReadyColor;
    [SerializeField] private Color _readyColor;
    [SerializeField] private Color _winningColor;

    public override void GenerateUI(Transform parent, EGenerateOption options)
    {
        base.GenerateUI(parent, options);

        _backGroundImage = transform.Find("PlayerIcon").GetComponent<Image>();
        _playerHeadImage = transform.Find("PlayerIcon/PlayerHeadImage").GetComponent<Image>();
        _playerNickNameTMP = transform.Find("NickNameText").GetComponent<TextMeshProUGUI>();
        _readyTMP = transform.Find("ReadyText").GetComponent<TextMeshProUGUI>();

        _ready = false;
        _backGroundImage.color = _unReadyColor;
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

    public void Winning()
    {
        _backGroundImage.color = _winningColor;
    }

    public void ReadyToggle()
    {
        _ready = !_ready;
        _backGroundImage.color = _ready ? _readyColor : _unReadyColor;
        _readyTMP.text = _ready ? "READY" : "";
    }
}