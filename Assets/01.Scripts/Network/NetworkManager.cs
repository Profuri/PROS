using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using Random = UnityEngine.Random;

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
        
    
    private PhotonView _photonView;
    private List<RoomInfo> _roomList;
    
    public event Action OnConnectedToMasterEvent;
    public event Action OnJoinedRoomEvent;
    public event Action OnJoinedLobbyEvent;
    public event Action OnLeftRoomEvent;
    public event Action<List<RoomInfo>> OnRoomListUpdateEvent;
    public event Action<Player> OnPlayerEnteredRoomEvent;
    public void Init()
    {
        _photonView = GetComponent<PhotonView>();
        OnConnectedToMasterEvent += () => Debug.Log("OnConnectedToMaster");
        OnJoinedLobbyEvent += () => Debug.Log("OnJoinedLobby");
        OnJoinedRoomEvent += () => Debug.Log("OnJoinedRoom");
        OnRoomListUpdateEvent += (list) => _roomList = list;
        
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        OnConnectedToMasterEvent?.Invoke();
        PhotonNetwork.JoinLobby();
    }

    public void CreateRoom() => PhotonNetwork.CreateRoom($"Room: {Random.Range(0,9999)}");
    public void LeaveRoom() => PhotonNetwork.LeaveRoom();
    public override void OnJoinedLobby() => OnJoinedLobbyEvent?.Invoke();
    public override void OnJoinedRoom() => OnJoinedRoomEvent?.Invoke();
    public override void OnLeftRoom() => OnLeftRoomEvent?.Invoke();
    public override void OnMasterClientSwitched(Player newMasterClient){ }
    public override void OnCreateRoomFailed(short returnCode, string message){ }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        OnPlayerEnteredRoomEvent?.Invoke(newPlayer);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate");
        OnRoomListUpdateEvent?.Invoke(roomList);
    }
}
