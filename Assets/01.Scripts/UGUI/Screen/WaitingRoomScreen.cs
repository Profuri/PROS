using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class WaitingRoomScreen : UGUIComponent
{
    [SerializeField] private InputSO _inputSO;
    [SerializeField] private Transform _playerCardParent;

    private Dictionary<Player, PlayerCard> _playerCardDiction;

    public override void GenerateUI(Transform parent, EGenerateOption options)
    {
        base.GenerateUI(parent, options);

        _playerCardDiction = new Dictionary<Player, PlayerCard>();

        _inputSO.OnEnterPress += EnterCallBack;
        _inputSO.OnBackPress += ExitCallBack;
        NetworkManager.Instance.OnPlayerEnteredRoomEvent += EnterPlayerCallBack;
        NetworkManager.Instance.OnPlayerLeftRoomEvent += LeftPlayerCallBack;
        
        EnterPlayerCallBack(NetworkManager.Instance.LocalPlayer);
    }

    public override void RemoveUI()
    {
        base.RemoveUI();

        _inputSO.OnEnterPress -= EnterCallBack;
        _inputSO.OnBackPress -= ExitCallBack;
        NetworkManager.Instance.OnPlayerEnteredRoomEvent -= EnterPlayerCallBack;
        NetworkManager.Instance.OnPlayerLeftRoomEvent -= LeftPlayerCallBack;
    }

    public override void UpdateUI()
    {
        // Do Nothing
    }

    #region CallBacks

    private void EnterCallBack()
    {
        
    }

    private void ExitCallBack()
    {
        
    }

    private void EnterPlayerCallBack(Player player)
    {
        Debug.Log($"Join player: {player.NickName}");
        var playerCard = UIManager.Instance.GenerateUGUI("PlayerCard", EGenerateOption.NONE, _playerCardParent) as PlayerCard;

        if (playerCard is null)
        {
            Debug.LogWarning("Something wrong when create player card");
            return;
        }
        
        // playerCard.SetColor();
        playerCard.SetNickName(player.NickName);
        
        _playerCardDiction.Add(player, playerCard);
    }

    private void LeftPlayerCallBack(Player player)
    {
        _playerCardDiction[player].RemoveUI();
        _playerCardDiction.Remove(player);
    }
    
    #endregion
}