using System.Collections.Generic;
using System.Linq;
using MonoPlayer;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine;

public class WaitingRoomScreen : UGUIComponent
{
    [SerializeField] private InputSO _inputSO;
    [SerializeField] private Transform _playerCardParent;

    private Dictionary<Player, PlayerCard> _playerCardDiction;
    private bool _startedRoom;

    public override void GenerateUI(Transform parent, EGenerateOption options)
    {
        base.GenerateUI(parent, options);

        _playerCardDiction = new Dictionary<Player, PlayerCard>();
        _startedRoom = false;

        _inputSO.OnEnterPress += ReadyCallBack;
        _inputSO.OnBackPress += ExitCallBack;
        NetworkManager.Instance.OnJoinedRoomEvent += JoinRoomCallBack;
        NetworkManager.Instance.OnPlayerEnteredRoomEvent += EnterPlayerCallBack;
        NetworkManager.Instance.OnPlayerLeftRoomEvent += LeftPlayerCallBack;
    }

    public override void RemoveUI()
    {
        base.RemoveUI();
        
        foreach(var pair in _playerCardDiction)
        {
            pair.Value.RemoveUI();
        }
        _playerCardDiction.Clear();

        _inputSO.OnEnterPress -= ReadyCallBack;
        _inputSO.OnBackPress -= ExitCallBack;
        NetworkManager.Instance.OnJoinedRoomEvent -= JoinRoomCallBack;
        NetworkManager.Instance.OnPlayerEnteredRoomEvent -= EnterPlayerCallBack;
        NetworkManager.Instance.OnPlayerLeftRoomEvent -= LeftPlayerCallBack;

        if (!_startedRoom)
        {
            NetworkManager.Instance.LeaveRoom();
        }
    }

    public override void UpdateUI()
    {
        // Do Nothing
    }

    public void ReadyPlayer(Player player)
    {
        _playerCardDiction[player].ReadyToggle();
        
        if (_playerCardDiction.Any(pair => !pair.Value.Ready))
        {
            return;
        }

        StartGame();
    }

    private void StartGame()
    {
        if (_playerCardDiction.Count < 2)
        {
            return;
        }

        if (NetworkManager.Instance.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;

            NetworkManager.Instance.LoadScene(ESceneName.Game);
        }
        
        var loading = UIManager.Instance.GenerateUGUI("LoadingScreen", EGenerateOption.STACKING | EGenerateOption.RESETING_POS) as LoadingScreen;
        loading.ExecuteLoading(ELoadingType.START_GAME, () =>
        {
            _startedRoom = true;
            UIManager.Instance.ClearPanel();
        });
    }

    #region CallBacks

    private void ReadyCallBack()
    {
        PlayerManager.Instance.ReadyPlayer(NetworkManager.Instance.LocalPlayer);
    }

    private void ExitCallBack()
    {
        UIManager.Instance.RemoveTopUGUI();
        UIManager.Instance.RemoveTopUGUI();
        UIManager.Instance.GenerateUGUI("OnlineMenuScreen", EGenerateOption.STACKING | EGenerateOption.CLEAR_PANEL | EGenerateOption.RESETING_POS);
    }

    private void JoinRoomCallBack()
    {
        var playerList = NetworkManager.Instance.PlayerList;
        foreach (var p in playerList)
        {
            EnterPlayerCallBack(p);
        }
    }

    private void EnterPlayerCallBack(Player player)
    {
        foreach (var pair in _playerCardDiction.Where(pair => pair.Value.Ready))
        {
            pair.Value.ReadyToggle();
        }
        
        var playerCard = UIManager.Instance.GenerateUGUI("PlayerCard", EGenerateOption.NONE, _playerCardParent) as PlayerCard;

        if (playerCard is null)
        {
            Debug.LogWarning("Something wrong when create player card");
            return;
        }

        var r = (float)player.CustomProperties["R"];
        var g = (float)player.CustomProperties["G"];
        var b = (float)player.CustomProperties["B"];
        var color = new Color(r, g, b, 1);
        
        playerCard.SetColor(color);
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