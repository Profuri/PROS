using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using System.Linq;
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
    
    private Collider2D _currentPlayer;
    private float _curOccupationTime;
    private OccupationArea _areaObj;
    private OccupationStageSystem _stageSystem;
    

    public OccupationSystem(OccupationStageSystem stageSystem, OccupationStruct occupationData)
    {
        _stageSystem = stageSystem;
        _occupationData = occupationData;

        if (_coroutine != null)
        {
            _stageSystem.StopCoroutine(_coroutine);
        }
        _coroutine = _stageSystem.StartCoroutine(DetectPlayers());
    }

    public void Init()
    {
        _stageSystem.StopAllCoroutines();
        PoolManager.Instance.Push(_areaObj);
    }

    public void SetOccupationPos(Vector3 targetPos)
    {
        _occupationPos = targetPos;

        if(_areaObj != null)
            PoolManager.Instance.Push(_areaObj);

        _areaObj = PoolManager.Instance.Pop("OccupationArea") as OccupationArea;
        _areaObj.transform.position = targetPos;
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
            Debug.Log($"Timer: {timer}");
            Debug.Log($"CurOccupationTime: {_curOccupationTime}");
            Debug.Log($"CurrentPlayer: {_currentPlayer}");
            timer += Time.deltaTime;
            _cols = Physics2D.OverlapCircleAll(_occupationPos,radius,layer);

            if (_curOccupationTime >= targetTime)
            {
                if(_currentPlayer == null) Debug.LogError($"Player is null");
                Debug.Log($"Winner: {_currentPlayer}");
            }
            
            if (_cols.Length > 0)
            {
                if (_cols.Length == 1)
                {
                    //var player = _cols[0].transform.root.GetComponentInChildren<PlayerBrain>().PhotonView.Owner;
                    var player = _cols[0];
                    if (_currentPlayer != player)
                    {
                        _currentPlayer = player;
                        _curOccupationTime = 0;
                    }
                    _curOccupationTime += (Time.deltaTime);
                }
            }
            yield return null;
        }
        _stageSystem.OnTargetChangeTime?.Invoke();
    }
}
