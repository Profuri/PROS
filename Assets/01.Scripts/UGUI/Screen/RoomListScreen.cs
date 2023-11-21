using System.Collections.Generic;
using UnityEngine;

public class RoomListScreen : UGUIComponent
{
    [SerializeField] private InputSO _inputSO;
    [SerializeField] private Transform _roomCardParent;

    private List<RoomCard> _roomCards;

    public override void GenerateUI(Transform parent, EGenerateOption options)
    {
        base.GenerateUI(parent, options);

        _roomCards = new List<RoomCard>();
        RefreshCallBack();
        
        _inputSO.OnBackPress += BackCallBack;
        _inputSO.OnRefreshPress += RefreshCallBack;
        NetworkManager.Instance.OnPlayerPropertiesUpdateEvent += RefreshCallBack;
    }

    public override void RemoveUI()
    {
        base.RemoveUI();

        _inputSO.OnBackPress -= BackCallBack;
        _inputSO.OnRefreshPress -= RefreshCallBack;
        NetworkManager.Instance.OnPlayerPropertiesUpdateEvent -= RefreshCallBack;
    }

    public override void UpdateUI()
    {
        // Do Nothing
    }
    
    #region CallBacks

    private void EnterRoomCallBack(int idx)
    {
        Debug.Log($"{idx}룸 입장");
        var room = NetworkManager.Instance.RoomList[idx];
        UIManager.Instance.GenerateUGUI("WaitingRoomScreen",
            EGenerateOption.CLEAR_PANEL | EGenerateOption.STACKING | EGenerateOption.RESETING_POS);
        var loading = UIManager.Instance.GenerateUGUI("LoadingScreen", EGenerateOption.STACKING | EGenerateOption.RESETING_POS) as LoadingScreen;
        loading.ExecuteLoading(ELoadingType.JOIN_ROOM);
        NetworkManager.Instance.JoinRoom(room.Name);
    }
        
    private void RefreshCallBack()
    {
        if (_roomCards.Count > 0)
        {
            foreach (var roomCard in _roomCards)
            {
                roomCard.RemoveUI();
            }
            _roomCards.Clear();
        }


        
        var roomList = NetworkManager.Instance.RoomList;
        for (var i = 0; i < roomList.Count; i++)
        {
            var cp = roomList[i].CustomProperties;
            var roomCard = UIManager.Instance.GenerateUGUI("RoomCard", EGenerateOption.NONE, _roomCardParent) as RoomCard;

            if (roomCard == null)
            {
                return;
            }

            roomCard.SetMode((EStageMode)cp["Mode"]);
            
            roomCard.SetTitle(roomList[i].Name);
            roomCard.SetPlayerCnt(roomList[i].PlayerCount, roomList[i].MaxPlayers);

            var idx = i;
            roomCard.SetButtonCallBack(() => EnterRoomCallBack(idx));

            _roomCards.Add(roomCard);
        }
    }

    private void BackCallBack()
    {
        UIManager.Instance.ReturnUGUI();
    }
    
    #endregion
}