using System;
using System.Collections.Generic;
using System.Reflection;
using Photon.Realtime;
using UnityEngine;
using MonoPlayer;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }
    
    
    #region INTERNAL VARIABLE
    
    [SerializeField]
    private PoolingListSO _poolingListSO;
    
    private Coroutine _gameCoroutine;
    #endregion

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
        
        NetworkManager.Instance.Init();
        SceneManagement.Instance.Init(this.transform);
        PlayerManager.Instance.Init();
        ScoreManager.Instance.Init();
        PoolManager.Instance = new PoolManager(this.transform);
        StageManager.Instance.Init();
        ItemManager.Instance.Init();

        _poolingListSO.pairs.ForEach(p => PoolManager.Instance.CreatePool(p.prefab,p.count));
    }

    public void OTCPlayer(Player player, Vector3 attackDir)
    {
        var playerBrain = PlayerManager.Instance.BrainDictionary[player];
        playerBrain.PlayerOTC.Damaged(attackDir);

        if (playerBrain.PlayerDefend.IsDefendBounce)
        {
            playerBrain.PlayerDefend.IsDefendBounce = false;
        }
    }
}

