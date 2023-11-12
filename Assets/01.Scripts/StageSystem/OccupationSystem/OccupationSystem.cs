using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using System.Linq;
using Photon.Pun;

public struct OccupationStruct
{
    public float TargetOccupationTime;
    public float MinChangeTime;
    public float MaxChangeTime;
    public float Radius;
    public int TargetLayer;

    public OccupationStruct(float targetOccupationTime, float minChangeTime, float maxChangeTime, float radius,
        int targetLayer)
    {
        TargetOccupationTime = targetOccupationTime;
        MinChangeTime = minChangeTime;
        MaxChangeTime = maxChangeTime;
        Radius = radius;
        TargetLayer = targetLayer;
    }
}

public class OccupationSystem
{
    private readonly OccupationStruct _occupationData;
    
    private Vector3 _occupationPos;
    private Collider2D[] _cols;
    private Coroutine _coroutine;
    
    private Player _currentPlayer;
    private float _curOccupationTime;
    private OccupationArea _areaObj;
    private OccupationStageSystem _stageSystem;
    
    public OccupationSystem(OccupationStageSystem stageSystem, OccupationStruct occupationData)
    {
        _stageSystem = stageSystem;
        _occupationData = occupationData;

        _coroutine = GameManager.Instance.StartCoroutine(DetectPlayers());
    }

    public void Init()
    {
        if(_coroutine != null)
        {
            GameManager.Instance.StopCoroutine(_coroutine);
        }
    }

    public void SetOccupationPos(Vector3 targetPos)
    {
        if (NetworkManager.Instance.IsMasterClient == false) return;
        _occupationPos = targetPos;
        if (_areaObj == null)
        {
            _areaObj = PhotonNetwork.Instantiate("OccupationArea", targetPos, Quaternion.identity)
                .GetComponent<OccupationArea>();
            _areaObj.Init();
        }
        else
        {
            _areaObj.transform.position = targetPos;
        }
    }

    private IEnumerator DetectPlayers()
    {
        float timer = 0f;
        float changeTime = Random.Range(_occupationData.MinChangeTime, _occupationData.MaxChangeTime);
        float targetTime = _occupationData.TargetOccupationTime;
        
        float radius = _occupationData.Radius;
        int layer = _occupationData.TargetLayer;

        _currentPlayer = null;
        _curOccupationTime = 0;
        
        while (timer <= changeTime)
        {
            if (_areaObj != null)
            {
                _areaObj.SetValue(_curOccupationTime / targetTime);
            }
            else
            {
                Debug.LogError("Area obj is null!");   
            }
            timer += Time.deltaTime;
            _cols = Physics2D.OverlapCircleAll(_occupationPos,radius,layer);

            if (_curOccupationTime >= targetTime)
            {
                if(_currentPlayer == null) Debug.LogError($"Player is null");
                _stageSystem.OnPlayerWinEvent?.Invoke(_currentPlayer);
            }
            
            if (_cols.Length > 0)
            {
                if (_cols.Length == 1)
                {
                    var player = _cols[0].transform.root.GetComponentInChildren<PlayerBrain>().PhotonView.Owner;
                    //var player = _cols[0];
                    if (_currentPlayer != player)
                    {
                        _currentPlayer = player;
                        _curOccupationTime = 0;
                    }
                    _curOccupationTime += (Time.deltaTime);
                }
            }
            else
            {
                _currentPlayer = null;
                _curOccupationTime = 0;
            }
            yield return null;
        }
        _stageSystem.OnTargetChangeTime?.Invoke();
    }
}
