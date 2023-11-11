using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class StageManager : MonoBehaviourPunCallbacks
{
    private static StageManager _instance;
    public static StageManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<StageManager>();
            }
            return _instance;
        }
    }
    
    private readonly List<BaseStageSystem> _stageSystems = new List<BaseStageSystem>();
    
    [SerializeField] private int _stageTypeCnt;
    public int StageTypeCnt => _stageTypeCnt;

    private BaseStageSystem _curStage = null;
    public BaseStageSystem CurStage => _curStage;

    public void Init()
    {
        _stageSystems.Add(GetComponent<NormalStageSystem>());
        _stageSystems.Add(GetComponent<OccupationStageSystem>());
        SceneManagement.Instance.OnGameSceneLoaded += GenerateStage;
    }

    private void Update()
    {
        _curStage?.StageUpdate();
    }

    private void GenerateStage()
    {
        if (!NetworkManager.Instance.IsMasterClient)
            return;
        
        Debug.Log("GenerateStage");

        //var stageIndex = Random.Range(0, _stageSystems.Count);
        var stageIndex = 1;
        //var mapIndex = Random.Range(0, StageManager.Instance.StageTypeCnt) + 1;
        var mapIndex = 3;
        
        NetworkManager.Instance.PhotonView.RPC("GenerateStageRPC", RpcTarget.All, stageIndex, mapIndex);
    }
    
    [PunRPC]
    private void GenerateStageRPC(int stageIndex, int mapIndex)
    {
        var nextStage = _stageSystems[stageIndex];
        
        if (nextStage == null)
        {
            Debug.LogError($"Can't find stage System in this index: {stageIndex}");
            return;
        }
        
        if (_curStage != null)
        {
            _curStage.StageLeave();
            _curStage = null;
        }

        _curStage = nextStage;
        
        _curStage.Init(mapIndex);
    }
}