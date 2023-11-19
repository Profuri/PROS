using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : UGUIComponent
{
    [SerializeField] private List<RectTransform> _graphicObjects;
    [SerializeField] private float _graphicSpeed;
    [SerializeField] private float _sineAreaSize;
    [SerializeField] private float _offset;
    [SerializeField] private float _originPosY;
    
    private float _theta = 0;
    private Action _loadingCompleteCallBack = null;

    public override void GenerateUI(Transform parent, EGenerateOption options)
    {
        _theta = 0;
        _graphicObjects.ForEach(rTrm => rTrm.anchoredPosition = new Vector2(rTrm.anchoredPosition.x, _originPosY));
        base.GenerateUI(parent, options);
    }
    
    public void ExecuteLoading(ELoadingType type, Action CompleteCallBack = null)
    {
        _loadingCompleteCallBack += CompleteCallBack;
        
        switch (type)
        {
            case ELoadingType.SERVER_CONNECT:
                NetworkManager.Instance.OnJoinedLobbyEvent += OnLoadingComplete;
                _loadingCompleteCallBack += () => NetworkManager.Instance.OnJoinedLobbyEvent -= OnLoadingComplete;
                break;
            case ELoadingType.JOIN_ROOM:
                NetworkManager.Instance.OnJoinedRoomEvent += OnLoadingComplete;
                _loadingCompleteCallBack += () => NetworkManager.Instance.OnJoinedRoomEvent -= OnLoadingComplete;
                break;
            case ELoadingType.REFRESH_ROOM:
                NetworkManager.Instance.OnRoomListUpdateEvent += OnLoadingComplete;
                _loadingCompleteCallBack += () => NetworkManager.Instance.OnRoomListUpdateEvent -= OnLoadingComplete;
                break;
        }
    }

    private void OnLoadingComplete()
    {
        UIManager.Instance.RemoveTopUGUI();
        _loadingCompleteCallBack?.Invoke(); 
        _loadingCompleteCallBack = null;
    }

    public override void UpdateUI()
    {
        _theta += _graphicSpeed * Time.deltaTime;

        for (var i = 0; i < _graphicObjects.Count; i++)
        {
            var theta = _theta + i * _offset;
            var sineValue = Mathf.Sin(theta * Mathf.Deg2Rad) * _sineAreaSize;
            _graphicObjects[i].anchoredPosition = new Vector2(_graphicObjects[i].anchoredPosition.x, _originPosY) + new Vector2(0, sineValue);
        }
    }
}