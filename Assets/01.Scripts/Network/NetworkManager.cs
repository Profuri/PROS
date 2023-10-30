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
    }
    
    private PhotonView _photonView;
    
    public event Action OnConnectedToMasterEvent;
    public event Action OnJoinedRoomEvent;
    public event Action OnJoinedLobbyEvent;
    public event Action<List<RoomInfo>> OnRoomListUpdateEvent;
    public event Action<Player> OnPlayerEnteredRoomEvent;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
        Init();
    }
    private void Init()
    {
        _photonView = GetComponent<PhotonView>();
        OnConnectedToMasterEvent += () => Debug.Log("OnConnectedToMaster");
        OnJoinedLobbyEvent += () => Debug.Log("OnJoinedLobby");
        OnJoinedRoomEvent += () => Debug.Log("OnJoinedRoom");
        
        PhotonNetwork.ConnectUsingSettings(); 
    }
    public override void OnConnectedToMaster()
    {
        OnConnectedToMasterEvent?.Invoke();
        PhotonNetwork.JoinLobby();
    }

    public void CreateRoom() => PhotonNetwork.CreateRoom($"Room: {Random.Range(0,9999)}");
    
    public override void OnJoinedLobby() => OnJoinedLobbyEvent?.Invoke();
    public override void OnJoinedRoom() => OnJoinedRoomEvent?.Invoke();
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        OnPlayerEnteredRoomEvent?.Invoke(newPlayer);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        OnRoomListUpdateEvent?.Invoke(roomList);
    }
}
