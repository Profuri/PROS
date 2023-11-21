using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class RoundWinnerScreen : UGUIComponent
{
    [SerializeField] private float _activeTime;
    private float _currentTime;

    [SerializeField] private float _leftLimit;
    [SerializeField] private float _rightLimit;
    
    private TextMeshProUGUI _upperWinnerNameTMP;
    private TextMeshProUGUI _downWinnerNameTMP;

    private Transform _playerCardPanel;

    private Dictionary<Player, PlayerCard> _playerCardDiction;

    public override void GenerateUI(Transform parent, EGenerateOption options)
    {
        base.GenerateUI(parent, options);

        _upperWinnerNameTMP = transform.Find("UpperRoundWinnerTMP").GetComponent<TextMeshProUGUI>();
        _downWinnerNameTMP = transform.Find("DownRoundWinnerTMP").GetComponent<TextMeshProUGUI>();
        
        _playerCardPanel = parent.Find("PlayerCardPanel");

        _upperWinnerNameTMP.rectTransform.anchoredPosition = new Vector2(_leftLimit, 430f);
        _downWinnerNameTMP.rectTransform.anchoredPosition = new Vector2(_rightLimit, -430f);
        
        _playerCardDiction = new Dictionary<Player, PlayerCard>();
        foreach (var player in NetworkManager.Instance.PlayerList)
        {
            var card = UIManager.Instance.GenerateUGUI("PlayerCard", EGenerateOption.NONE, _playerCardPanel) as PlayerCard;
            
            var r = (float)player.CustomProperties["R"];
            var g = (float)player.CustomProperties["G"];
            var b = (float)player.CustomProperties["B"];
            var color = new Color(r, g, b, 1);
            
            card.SetColor(color);
            card.SetNickName(player.NickName);

            if (card.Ready)
            {
                card.ReadyToggle();
            }
            
            _playerCardDiction.Add(player, card);
        }

        _currentTime = 0f;
    }

    public override void RemoveUI()
    {
        base.RemoveUI();

        foreach (var pair in _playerCardDiction)
        {
            pair.Value.RemoveUI();
        }
        _playerCardDiction.Clear();
    }

    public override void UpdateUI()
    {
        _currentTime += Time.deltaTime;
        var percent = _currentTime / _activeTime;

        if (percent >= 1f)
        {
            UIManager.Instance.RemoveTopUGUI();
            StageManager.Instance.RemoveCurMap();
            StageManager.Instance.GenerateNewMap();
        }

        _upperWinnerNameTMP.rectTransform.anchoredPosition =
            new Vector2(Mathf.Lerp(_leftLimit, _rightLimit, percent), 430f);
        _downWinnerNameTMP.rectTransform.anchoredPosition =
            new Vector2(Mathf.Lerp(_rightLimit, _leftLimit, percent), -430f);
    }

    public void SetWinner(Player player)
    {
        _downWinnerNameTMP.text = player.NickName;
        _playerCardDiction[player].Winning();
    }
}