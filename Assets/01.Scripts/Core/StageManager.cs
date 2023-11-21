using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
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

    private int _curMapIdx;
    public int MapIdx => _curMapIdx;

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

    public void RoundWinner(Player winner)
    {
        var winnerScreen = UIManager.Instance.GenerateUGUI("RoundWinnerScreen", EGenerateOption.STACKING | EGenerateOption.RESETING_POS) as RoundWinnerScreen;
        winnerScreen.SetWinner(winner);
        // NetworkManager.Instance.PhotonView.RPC(nameof(RoundWinnerRPC), RpcTarget.All, winner);
    }

    public void StageWinner(Player winner)
    {
        NetworkManager.Instance.PhotonView.RPC(nameof(StageWinnerRPC), RpcTarget.All, winner);
    }

    [PunRPC]
    public void StageWinnerRPC(Player winner)
    {
        var winnerScreen = UIManager.Instance.GenerateUGUI("StageWinnerScreen", EGenerateOption.STACKING | EGenerateOption.RESETING_POS) as StageWinnerScreen;
        winnerScreen.SetWinner(winner);
    }

    public void GenerateNewMap()
    {
        if (!NetworkManager.Instance.IsMasterClient)
            return;
        
        // var type = Random.Range(0, StageTypeCnt) + 1;
        var type = 1;
        NetworkManager.Instance.PhotonView.RPC(nameof(GenerateNewMapRPC), RpcTarget.All, type);
    }
    
    [PunRPC]
    public void GenerateNewMapRPC(int type)
    {
        _curStage.GenerateNewStage(type);
    }
    
    public void RemoveCurMap()
    {
        _curStage.RemoveCurStage();
    }

    private void GenerateNextStage()
    {
        if (!NetworkManager.Instance.IsMasterClient)
            return;

        //var stageIndex = Random.Range(0, _stageSystems.Count);
        int stageIndex = 0;
        var mapIndex = Random.Range(0, StageTypeCnt) + 1;
        
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
        _curMapIdx = mapIndex;
        
        _curStage.Init(mapIndex);
    }

    public void SetPosition(int index, Vector2 position)
    {
        NetworkManager.Instance.PhotonView.RPC(nameof(SetPositionRPC), RpcTarget.All, index, position);
    }

    [PunRPC]
    public void SetPositionRPC(int index, Vector2 position)
    {
        _curStage.CurStageObject.Platforms[index].transform.position = position;
    }
}