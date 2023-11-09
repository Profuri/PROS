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

    [SerializeField] private List<BaseStageSystem> _stageSystems;
    private BaseStageSystem _curStage = null;

    public void Init()
    {
        SceneManagement.Instance.OnGameSceneLoaded += GenerateStageRPC;
    }

    private void GenerateStageRPC()
    {
        var nextStage = _stageSystems[Random.Range(0, _stageSystems.Count)];
        NetworkManager.Instance.PhotonView.RPC("GenerateStage", RpcTarget.All, nextStage);
    }
    
    [PunRPC]
    private void GenerateStage(BaseStageSystem nextStage)
    {
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
        _curStage.Init();
    }
}