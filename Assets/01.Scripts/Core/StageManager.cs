using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
[System.Serializable]
public struct MapBoundStruct
{
    public float minX;
    public float minY;
    public float maxX;
    public float maxY;
}

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
    
    [SerializeField] private MapBoundStruct _mapBound;
    public MapBoundStruct MapBound => _mapBound;
    [SerializeField] private int _stageTypeCnt;
    public int StageTypeCnt => _stageTypeCnt;

    private BaseStageSystem _curStage = null;
    public BaseStageSystem CurStage => _curStage;

    public void Init()
    {
        _stageSystems.Add(GetComponent<NormalStageSystem>());
        _stageSystems.Add(GetComponent<OccupationStageSystem>());
        SceneManagement.Instance.OnGameSceneLoaded += GenerateNextStage;
    }

    private void Update()
    {
        _curStage?.StageUpdate();
    }

    public void GenerateNewMap()
    {
        var type = Random.Range(0, StageTypeCnt) + 1;
        NetworkManager.Instance.PhotonView.RPC("GenerateNewMapRPC", RpcTarget.All, type);
    }
    
    [PunRPC]
    public void GenerateNewMapRPC(int type)
    {
        _curStage.GenerateNewStage(type);
    }

    public void RemoveCurMap()
    {
        NetworkManager.Instance.PhotonView.RPC("RemoveCurMapRPC", RpcTarget.All);
    }
    
    [PunRPC]
    public void RemoveCurMapRPC()
    {
        _curStage.RemoveCurStage();
    }

    private void GenerateNextStage()
    {
        if (!NetworkManager.Instance.IsMasterClient)
            return;

        //var stageIndex = Random.Range(0, _stageSystems.Count);
        int stageIndex = 0;
        var mapIndex = Random.Range(0, StageManager.Instance.StageTypeCnt) + 1;
        
        NetworkManager.Instance.PhotonView.RPC("GenerateNextStageRPC", RpcTarget.All, stageIndex, mapIndex);
    }
    
    [PunRPC]
    private void GenerateNextStageRPC(int stageIndex, int mapIndex)
    {
        var nextStage = _stageSystems[stageIndex];
        
        if (nextStage == null)
        {
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