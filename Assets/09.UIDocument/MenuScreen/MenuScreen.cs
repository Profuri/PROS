using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuScreen : MonoBehaviour
{
    private UIDocument _uiDocument;

    [SerializeField] private VisualTreeAsset _playerTempalte;

    private VisualElement _popupContainer;
    private TextField _nameField;

    private VisualElement _panelBox;

    private VisualElement _lobbyPanel;
    private TextField _roomCodeField;

    private VisualElement _roomPanel;
    private Label _roomNameLabel;
    private ScrollView _roomPlayerList;
    private Button _startGameBtn;
    private Button _exitRoomBtn;

    private VisualElement _currentPanel;

    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        VisualElement root = _uiDocument.rootVisualElement;

        _popupContainer = root.Q<VisualElement>("popup-container");
        _popupContainer.RemoveFromClassList("off");

        _panelBox = root.Q<VisualElement>("panel-box");

        _nameField = root.Q<TextField>("name-field");
        root.Q<Button>("btn-confirm").RegisterCallback<ClickEvent>(HandleNameConfirm);

        _lobbyPanel = root.Q<VisualElement>("lobby-panel");
        _roomCodeField = root.Q<TextField>("room-name-field");
        root.Q<Button>("btn-create-room").RegisterCallback<ClickEvent>(HandleCreateRoom);
        root.Q<Button>("btn-join-room").RegisterCallback<ClickEvent>(HandleJoinRoom);

        _roomPanel = root.Q<VisualElement>("room-panel");
        _roomNameLabel = root.Q<Label>("room-name");
        _roomPlayerList = root.Q<ScrollView>("player-list");
        _startGameBtn = root.Q<Button>("btn-start");
        _exitRoomBtn = root.Q<Button>("btn-exit");
        _startGameBtn.RegisterCallback<ClickEvent>(HandleStartGame);
        _exitRoomBtn.RegisterCallback<ClickEvent>(HandleExitRoom);

        GoToLobbyPanel();

        NetworkManager.Instance.OnJoinedRoomEvent += HandleJoinedRoom;
        NetworkManager.Instance.OnLeftRoomEvent += HandleLeftRoom;
        NetworkManager.Instance.OnPlayerEnteredRoomEvent += AddPlayer;
        NetworkManager.Instance.OnPlayerLeftRoomEvent += RemovePlayer;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Instance == null) return;
        NetworkManager.Instance.OnJoinedRoomEvent -= HandleJoinedRoom;
        NetworkManager.Instance.OnLeftRoomEvent -= HandleLeftRoom;
        NetworkManager.Instance.OnPlayerEnteredRoomEvent -= AddPlayer;
        NetworkManager.Instance.OnPlayerLeftRoomEvent -= RemovePlayer;
    }

    private void HandleLeftRoom()
    {
        GoToLobbyPanel();
    }

    private void GoToRoomPanel()
    {
        _panelBox.style.left = new Length(-100, LengthUnit.Percent);
    }

    private void GoToLobbyPanel()
    {
        _panelBox.style.left = new Length(0, LengthUnit.Percent);
    }

    private void HandleNameConfirm(ClickEvent evt)
    {
        _popupContainer.AddToClassList("off");
        NetworkManager.Instance.LocalPlayer.NickName = _nameField.text;
    }

    private void HandleCreateRoom(ClickEvent evt)
    {
        NetworkManager.Instance.CreateRoom();
    }

    private void HandleJoinRoom(ClickEvent evt)
    {
        NetworkManager.Instance.JoinRoom($"Room: {_roomCodeField.value}");
    }

    private void HandleJoinedRoom()
    {
        GoToRoomPanel();
        _roomNameLabel.text = NetworkManager.Instance.GetCurRoom.Name;
        _roomPlayerList.Clear();
        NetworkManager.Instance.PlayerList.ForEach(p => AddPlayer(p));
        if(!PhotonNetwork.IsMasterClient)
        {
            _startGameBtn.pickingMode = PickingMode.Ignore;
            _startGameBtn.AddToClassList("off");
        }
        else
        {
            _startGameBtn.pickingMode = PickingMode.Position;
            _startGameBtn.RemoveFromClassList("off");
        }
    }

    private void HandleStartGame(ClickEvent evt)
    {
        Debug.Log("Game Start");
        // Start Game
        if (NetworkManager.Instance.IsMasterClient)
        {
            NetworkManager.Instance.LoadScene(ESceneName.Game);
        }
    }

    private void HandleExitRoom(ClickEvent evt)
    {
        if(NetworkManager.Instance.GetCurRoom != null)
        NetworkManager.Instance.LeaveRoom();
        GoToLobbyPanel();
    }

    public void AddPlayer(Player player)
    {
        VisualElement newPlayer = _playerTempalte.Instantiate();
        newPlayer.name = player.NickName;
        newPlayer.Q<Label>("player-name").text = player.NickName;
        _roomPlayerList.Add(newPlayer);
    }

    private void RemovePlayer(Player player)
    {
        VisualElement e = _roomPlayerList.Q<VisualElement>(player.NickName);
        if(e != null)
        {
            _roomPlayerList.Remove(e);
        }
    }
}
