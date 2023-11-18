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

    private void EnterRoomCallBack()
    {
        Debug.Log("입장");
    }

    private void RefreshCallBack()
    {
        foreach (var roomCard in _roomCards)
        {
            roomCard.RemoveUI();
        }
        _roomCards.Clear();
        
        var roomList = NetworkManager.Instance.RoomList;
        foreach (var room in roomList)
        {
            var cp = room.CustomProperties;
            var roomCard = UIManager.Instance.GenerateUGUI("RoomCard", EGenerateOption.NONE, _roomCardParent) as RoomCard;

            if (roomCard == null)
            {
                return;
            }

            // roomCard.SetMode((EStageMode)cp["Mode"]);
            
            roomCard.SetTitle(room.Name);
            roomCard.SetPlayerCnt(room.PlayerCount, room.MaxPlayers);
            roomCard.SetButtonCallBack(EnterRoomCallBack);
        }
    }

    private void BackCallBack()
    {
        UIManager.Instance.ReturnUGUI();
    }
    
    #endregion
}