using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

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
        _stageSystems.Add(new NormalStageSystem(EStageMode.NORMAL));
        SceneManagement.Instance.OnGameSceneLoaded += GenerateStage;
    }

    private void GenerateStage()
    {
        if (!NetworkManager.Instance.IsMasterClient)
            return;

        var index = Random.Range(0, _stageSystems.Count);
        NetworkManager.Instance.PhotonView.RPC("GenerateStageRPC", RpcTarget.All, index);
    }
    
    [PunRPC]
    private void GenerateStageRPC(int index)
    {
        var nextStage = _stageSystems[index];
        
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
        
        var type = Random.Range(0, StageManager.Instance.StageTypeCnt) + 1;
        _curStage.Init(type);
    }
}