using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using System.Linq;
using ExitGames.Client.Photon;

public enum ESceneName
{
    Menu,Game,End
}
public class NetworkManager : MonoBehaviourPunCallbacks
{
    private static NetworkManager _instance;
    public static NetworkManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<NetworkManager>();
            }
            return _instance;
        }
        set => _instance = value;
    }

    public RoomInfo GetCurRoom => PhotonNetwork.CurrentRoom;
    public bool IsMasterClient => PhotonNetwork.IsMasterClient;
    public Player LocalPlayer => PhotonNetwork.LocalPlayer;
    public List<RoomInfo> RoomList => _roomList;
    public List<Player> PlayerList => PhotonNetwork.PlayerList.ToList();

    private PhotonView _photonView;
    public PhotonView PhotonView => _photonView;
    private List<RoomInfo> _roomList;

    public event Action OnConnectedToMasterEvent;
    public event Action OnJoinedRoomEvent;
    public event Action OnJoinedLobbyEvent;
    public event Action OnLeftRoomEvent;
    public event Action OnPlayerPropertiesUpdateEvent;
    public event Action OnRoomListUpdateEvent;
    public event Action<Player> OnPlayerEnteredRoomEvent;
    public event Action<Player> OnPlayerLeftRoomEvent;
    
    public void Init()
    {
        _photonView = GetComponent<PhotonView>();
        OnConnectedToMasterEvent += () => Debug.Log("OnConnectedToMaster");
        OnJoinedLobbyEvent += () => Debug.Log("OnJoinedLobby");
        OnJoinedRoomEvent += () => Debug.Log("OnJoinedRoom");
        // OnRoomListUpdateEvent += (list) => _roomList = list;
        
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnConnectedToMaster()
    {
        OnConnectedToMasterEvent?.Invoke();
        PhotonNetwork.JoinLobby();
    }

    public void CreateRoom(Player owner, EStageMode mode)
    {
        var options = new RoomOptions
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 4,
        };
        Hashtable modeInfo = new Hashtable { { "Mode", (int)mode } };
        string[] customRoomPropsForLobby = {"Mode" };
        options.CustomRoomProperties = modeInfo;
        options.CustomRoomPropertiesForLobby = customRoomPropsForLobby;

        PhotonNetwork.CreateRoom($"{owner.NickName}'S ROOM", options);
    }

    public void LeaveRoom() => PhotonNetwork.LeaveRoom();
    public void JoinRoom(string name) => PhotonNetwork.JoinRoom(name);
    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) => OnPlayerPropertiesUpdateEvent?.Invoke();
    public override void OnJoinedLobby() => OnJoinedLobbyEvent?.Invoke();
    public override void OnJoinedRoom() => OnJoinedRoomEvent?.Invoke();
    public override void OnLeftRoom() => OnLeftRoomEvent?.Invoke();
    public override void OnMasterClientSwitched(Player newMasterClient){ }
    public override void OnCreateRoomFailed(short returnCode, string message){ }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    { 
        OnPlayerEnteredRoomEvent?.Invoke(newPlayer);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        OnPlayerLeftRoomEvent?.Invoke(otherPlayer);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate");
        _roomList = roomList;
        OnRoomListUpdateEvent?.Invoke();
    }

    public void LoadScene(ESceneName sceneType)
    {
        PhotonNetwork.LoadLevel(sceneType.ToString());
    }
}