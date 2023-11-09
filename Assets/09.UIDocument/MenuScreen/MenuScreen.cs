using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public enum Panel
{
    Lobby = 0, Find = 1, Room = 2
}

public class MenuScreen : MonoBehaviour
{
    private UIDocument _uiDocument;

    [SerializeField] private VisualTreeAsset _playerTempalte;
    [SerializeField] private VisualTreeAsset _roomTemplate;

    private readonly string _nameKey = "PLAYER_NAME";

    private Panel _currentPanel;

    private VisualElement _content;

    // Popup Panel
    private VisualElement _popupPanel;
    private TextField _nameField;

    // Lobby
    private Button _createBtn;
    private Button _findBtn;

    // Find
    private ScrollView _roomList;
    private TextField _joinCodeField;
    private Button _joinCodeBtn;

    // Room
    private ScrollView _playerList;
    private Label _roomNameLabel;
    private Label _roomPlayerCount;
    private Button _startGameBtn;
    private Button _exitRoomBtn;
    private Dictionary<Player, VisualElement> _playerDictionary = new Dictionary<Player, VisualElement>();
    
    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        VisualElement root = _uiDocument.rootVisualElement;

        // Page Init
        _content = root.Q<VisualElement>("content");
        _content.style.left = new Length(0f, LengthUnit.Percent);
        _currentPanel = Panel.Lobby;

        #region Popup Panel
        _popupPanel = root.Q<VisualElement>("popup-panel");
        _popupPanel.RemoveFromClassList("off");
        _nameField = _popupPanel.Q<TextField>("name-field");
        _nameField.value = PlayerPrefs.GetString(_nameKey, "이름을 입력하세요.");
        _popupPanel.Q<Button>("btn-ok").RegisterCallback<ClickEvent>(HandleConfirmName);
        #endregion

        #region Lobby
        _createBtn = root.Q<Button>("btn-create");
        _createBtn.RegisterCallback<ClickEvent>(HandleCreateRoom);

        _findBtn = root.Q<Button>("btn-find");
        _findBtn.RegisterCallback<ClickEvent>(HandleFindRoom);
        #endregion

        #region Find
        _roomList = root.Q<ScrollView>("room-list");
        _roomList.Clear();

        _joinCodeField = root.Q<TextField>("join-code-field");
        _joinCodeBtn = root.Q<Button>("btn-join-code");
        _joinCodeBtn.RegisterCallback<ClickEvent>(HandleJoinCode);
        root.Q<Button>("btn-refresh").RegisterCallback<ClickEvent>(HandleRefresh);

        root.Q<Button>("btn-back").RegisterCallback<ClickEvent>(evt =>
        {
            ChangePanel(Panel.Lobby);
        });
        #endregion

        #region Room
        _playerList = root.Q<ScrollView>("player-list");
        _playerList.Clear();

        _roomNameLabel = _playerList.parent.Q<Label>("page-title");
        _roomPlayerCount = root.Q<Label>("player-count");

        _startGameBtn = root.Q<Button>("btn-start-game");
        _exitRoomBtn = root.Q<Button>("btn-exit-room");
        _startGameBtn.RegisterCallback<ClickEvent>(HandleStartGame);
        _exitRoomBtn.RegisterCallback<ClickEvent>(HandleExitRoom);
        #endregion

        #region Network Event
        NetworkManager.Instance.OnJoinedLobbyEvent += HandleJoinedLobby;
        NetworkManager.Instance.OnJoinedRoomEvent += HandleJoinedRoom;
        NetworkManager.Instance.OnLeftRoomEvent += HandleLeftRoom;
        NetworkManager.Instance.OnPlayerEnteredRoomEvent += HandlePlayerEnterRoom;
        NetworkManager.Instance.OnPlayerLeftRoomEvent += HandlePlayerLeftRoom;
        NetworkManager.Instance.OnRoomListUpdateEvent += HandleRoomListUpdate;
        #endregion
    }

    private void OnDisable()
    {
        
    }

    private void ChangePanel(Panel panel)
    {
        _currentPanel = panel;
        _content.style.left = new Length((float)panel * -100f, LengthUnit.Percent);
    }

    private void HandleConfirmName(ClickEvent evt)
    {
        if(string.IsNullOrEmpty(_nameField.value))
        {
            Debug.LogError("이름을 입력하세요.");
            return;
        }
        NetworkManager.Instance.LocalPlayer.NickName = _nameField.value;
        PlayerPrefs.SetString(_nameKey, _nameField.value);
        _popupPanel.AddToClassList("off");
    }

    private void HandleCreateRoom(ClickEvent evt)
    {
        if (_currentPanel != Panel.Lobby) return;
        NetworkManager.Instance.CreateRoom();
    }

    private void HandleFindRoom(ClickEvent evt)
    {
        if (_currentPanel != Panel.Lobby) return;
        ChangePanel(Panel.Find);
    }

    private void HandleJoinCode(ClickEvent evt)
    {
        if (NetworkManager.Instance.GetCurRoom != null)
        {
            Debug.LogError("잘못된 접근입니다.");
            return;
        }
        if (string.IsNullOrEmpty(_joinCodeField.value))
        {
            Debug.LogError("방 코드를 입력하세요.");
            return;
        }
        NetworkManager.Instance.JoinRoom($"Room: {_joinCodeField.value}");
    }

    private void HandleRefresh(ClickEvent evt)
    {
        HandleRoomListUpdate(NetworkManager.Instance.RoomList);
    }

    private void HandleExitRoom(ClickEvent evt)
    {
        if (NetworkManager.Instance.GetCurRoom == null)
        {
            Debug.LogError("잘못된 접근입니다.");
            return;
        }

        NetworkManager.Instance.LeaveRoom();
    }

    private void HandleStartGame(ClickEvent evt)
    {
        if (NetworkManager.Instance.GetCurRoom == null)
        {
            Debug.LogError("잘못된 접근입니다.");
            return;
        }
    }

    #region Network Event

    private void HandleRoomListUpdate(List<RoomInfo> updatedList)
    {
        _roomList.Clear();
        foreach(var room in NetworkManager.Instance.RoomList)
        {
            VisualElement newRoom = _roomTemplate.Instantiate();
            newRoom.Q<Label>("room-name").text = $"{room.masterClientId}'s Room";
            newRoom.Q<Button>("btn-join").RegisterCallback<ClickEvent>(evt =>
            {
                NetworkManager.Instance.JoinRoom(room.Name);
            });
            _roomList.Add(newRoom);
        }
    }

    private void HandlePlayerLeftRoom(Player other)
    {
        _playerList.Remove(_playerDictionary[other]);
        _playerDictionary.Remove(other);
    }

    private void HandlePlayerEnterRoom(Player other)
    {
        VisualElement newPlayer = _playerTempalte.Instantiate();
        newPlayer.Q<Label>("player-name").text = other.NickName;
        _playerList.Add(newPlayer);
        _playerDictionary.Add(other, newPlayer);
    }

    private void HandleLeftRoom()
    {
        ChangePanel(Panel.Find);

        HandleRoomListUpdate(NetworkManager.Instance.RoomList);
    }

    private void HandleJoinedRoom()
    {
        RoomInfo room = NetworkManager.Instance.GetCurRoom;
        _playerList.Clear();
        _playerDictionary.Clear();
        foreach(var player in NetworkManager.Instance.PlayerList)
        {
            VisualElement newPlayer = _playerTempalte.Instantiate();
            newPlayer.Q<Label>("player-name").text = player.NickName;
            if(player.IsMasterClient)
            {
                _roomNameLabel.text = $"{player.NickName}'s Room";
                newPlayer.Q<VisualElement>("master-icon").style.visibility = Visibility.Visible;
            }
            _playerList.Add(newPlayer);
            _playerDictionary.Add(player, newPlayer);
        }

        _roomPlayerCount.text = $"Join Code: {room.Name.Split(' ')[1]}";

        if(NetworkManager.Instance.IsMasterClient)
        {
            _startGameBtn.pickingMode = PickingMode.Position;
        }
        else
        {
            _startGameBtn.pickingMode = PickingMode.Ignore;
        }

        ChangePanel(Panel.Room);
    }

    private void HandleJoinedLobby()
    {
        _createBtn.pickingMode = PickingMode.Position;
        _findBtn.pickingMode = PickingMode.Position;
    }

    #endregion
}
