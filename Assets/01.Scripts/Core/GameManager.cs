using System;
using System.Collections.Generic;
using System.Reflection;
using Photon.Realtime;
using UnityEngine;
using MonoPlayer;
using Random = UnityEngine.Random;

public enum EPLAYER_STATE
{
    DEAD = -1,
    NORMAL = 0,
    WIN = 100,    
}
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


    public event Action<Player> OnGameEnd;
    public event Action<Player> OnRoundEnd;

    public event Action OnGameStart;
    public event Action OnRoundStart;
    
    #region INTERNAL VARIABLE
    [SerializeField] private int _targetWinCnt = 3;
    
    [SerializeField]
    private PoolingListSO _poolingListSO;
    
    private Dictionary<Player, int> _playerGameDictionary;
    private Dictionary<Player, int> _playerWinDictionary;
    
    private Coroutine _gameCoroutine;
    #endregion
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
        
        _playerGameDictionary = new Dictionary<Player, int>();
        _playerWinDictionary = new Dictionary<Player, int>();
        
        OnGameEnd += (value) => Debug.LogError($"GameEnd :{value}");        
        
        NetworkManager.Instance.Init();
        SceneManagement.Instance.Init(this.transform);
        PlayerManager.Instance.Init();
        ParticleManager.Instance.Init();
        PoolManager.Instance = new PoolManager(this.transform);

        _poolingListSO.pairs.ForEach(p => PoolManager.Instance.CreatePool(p.prefab,p.count));

        PlayerManager.Instance.OnAllPlayerLoad += GameStart;
        
        ScoreManager.Instance.Init();
    }

    private void GameStart()
    {
        Debug.Log("GameStart");
        
        //이거 소환하는거 PlayerManager에서 Dictionary에 추가하고 게임을 시작하는 방향이 더 맞을수도 있을 것 같음
        if (NetworkManager.Instance.IsMasterClient)
        {
            foreach (var player in PlayerManager.Instance.LoadedPlayerList)
            {
                Vector3 randomPos = new Vector3(Random.Range(-5f,-5f),0,0);
                
                PlayerBrain brain = PlayerManager.Instance.BrainDictionary[player];
                brain.Init(randomPos);
            }
        }

        _playerGameDictionary.Clear();
        foreach (var player in PlayerManager.Instance.LoadedPlayerList)
        {
            _playerGameDictionary.Add(player,(int)EPLAYER_STATE.NORMAL);
            _playerWinDictionary.Add(player,0);
        }
        
        OnGameStart?.Invoke();
    }

    public void OTCPlayer(Player player, Vector3 attackDir)
    {
        var playerBrain = PlayerManager.Instance.BrainDictionary[player];
        playerBrain.PlayerOTC.Damaged(attackDir);
    
        // switch (CurrentGameMode)
        // {
        //     case EGAME_MODE.DEATH_MATCH:
        //         _playerGameDictionary[player] = (int)EPLAYER_STATE.DEAD;
        //         break;
        //     case EGAME_MODE.AREA_SEIZE:
        //         // Todo : Player가 다시 소환 되어야 함
        //         break;
        // }
    }
}

