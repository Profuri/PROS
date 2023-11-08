using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using MonoPlayer;
using Random = UnityEngine.Random;

public enum EGAME_MODE
{
    DEATH_MATCH = 0,
    AREA_SEIZE = 1,
    END = 2
}

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
    
    private EGAME_MODE _currentGameMode = EGAME_MODE.DEATH_MATCH;
    public EGAME_MODE CurrentGameMode => _currentGameMode;

    #region INTERNAL VARIABLE
    [SerializeField] private int _targetWinCnt = 3;
    
    [SerializeField]
    private PoolingListSO _poolingListSO;
    
    private Dictionary<EGAME_MODE, MethodInfo> _gameMethodDictionary;
    
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
        
        //GameMode에 맞는 메소드 가져오기
        _gameMethodDictionary = new Dictionary<EGAME_MODE, MethodInfo>();
        _playerGameDictionary = new Dictionary<Player, int>();
        _playerWinDictionary = new Dictionary<Player, int>();

        foreach (EGAME_MODE gameMode in Enum.GetValues(typeof(EGAME_MODE)))
        {
            try
            {
                var methodInfo = this.GetType().GetMethod($"DoGameMode{gameMode.ToString()}",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                _gameMethodDictionary.Add(gameMode, methodInfo);
            }
            catch
            {
                Debug.LogError($"Can't Find this method from this game mode {gameMode}");
            }
        }

        OnRoundEnd += (value) => RoundStart();
        OnGameEnd += (value) => Debug.LogError($"GameEnd :{value}");        
        
        NetworkManager.Instance.Init();
        SceneManagement.Instance.Init(this.transform);
        PlayerManager.Instance.Init();
        ParticleManager.Instance.Init();
        PoolManager.Instance = new PoolManager(this.transform);

        _poolingListSO.pairs.ForEach(p => PoolManager.Instance.CreatePool(p.prefab,p.count));

        PlayerManager.Instance.OnAllPlayerLoad += GameStart;
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
        RoundStart();
    }
    
    public void RoundStart()
    {
        //플레이어의 상태 초기화
        //InvalidOperationException
        
        if (_gameCoroutine != null)
        {
            StopCoroutine(_gameCoroutine);
        }

        if (NetworkManager.Instance.IsMasterClient)
        {
            for (int i = 0; i < PlayerManager.Instance.LoadedPlayerList.Count; i++)
            {
                var player = PlayerManager.Instance.LoadedPlayerList[i];
                PlayerBrain pb = PlayerManager.Instance.BrainDictionary[player];
                Vector3 pos = Vector3.zero + new Vector3(i * 3 ,0, 0);
                pb.Init(Vector3.zero);
                pb.Rigidbody.velocity = Vector3.zero;
            }
        }
        
        foreach (var pair in _playerGameDictionary.ToList())
        {
            _playerGameDictionary[pair.Key] = (int)EPLAYER_STATE.NORMAL;
        }
        
        //var gameMode = GetRandomGameMode();
        //ChangeGameMode(gameMode);
        
        _gameCoroutine = StartCoroutine(GameCoroutine(EGAME_MODE.DEATH_MATCH));
        OnRoundStart?.Invoke();
    }
        
    #region GameSystem
    private IEnumerator GameCoroutine(EGAME_MODE eGameMode)
    {
        MethodInfo methodInfo = _gameMethodDictionary[eGameMode];
        while (true)
        {
            methodInfo?.Invoke(this, null);
            yield return null;
        }
    }
    
    private void DoGameModeDEATH_MATCH()
    {
        //현재 살아있는 플레이어를 전부 찾음
        var alivePlayers =
            from kvp in _playerGameDictionary
            where kvp.Value == (int)EPLAYER_STATE.NORMAL
            select kvp.Key;

        //살아있는 플레이어가 한 명이면 플레이어를 이겼다고 처리해준다.
        if (alivePlayers.Count() == 1)
        {
            Player winPlayer = alivePlayers.First();
            NetworkManager.Instance.PhotonView.RPC("ScorePlayerRPC",RpcTarget.All,winPlayer); 
            StopCoroutine(_gameCoroutine);
        }
    }
    
    private void DoGameModeAREA_SEIZE()
    {
        
                    
                    
        foreach (var kvp in _playerGameDictionary)
        {
            if (kvp.Value >= (int)EPLAYER_STATE.WIN)
            {
                //ScorePlayer(kvp.Key);
                //OnRoundEnd?.Invoke(kvp.Key);
                
                //StartCoroutine(Test(kvp.Key));
            }
        }
    }
    
    [PunRPC]
    private void ScorePlayerRPC(Player player)
    {
        _playerGameDictionary[player]++;

        foreach (var kvp in _playerGameDictionary)
        {
            Debug.LogError($"Key: {kvp.Key}Value: {kvp.Value}");
            if (kvp.Value >= _targetWinCnt)
            {
                OnGameEnd?.Invoke(kvp.Key);
                return;
            }
        }

        OnRoundEnd?.Invoke(player);
        //OnGameEnd?.Invoke(player);
        //OnRoundEnd?.Invoke(player);
    }
    
    public void OTCPlayer(Player player, Vector3 attackDir)
    {
        var playerBrain = PlayerManager.Instance.BrainDictionary[player];
        playerBrain.PlayerOTC.Damaged(attackDir);

        switch (CurrentGameMode)
        {
            case EGAME_MODE.DEATH_MATCH:
                _playerGameDictionary[player] = (int)EPLAYER_STATE.DEAD;
                break;
            case EGAME_MODE.AREA_SEIZE:
                // Todo : Player가 다시 소환 되어야 함
                break;
        }
    }
    
    public void ChangeGameMode(EGAME_MODE gameMode) => _currentGameMode = gameMode;
    public EGAME_MODE GetRandomGameMode() => (EGAME_MODE)Random.Range(0,(int)EGAME_MODE.END);
    #endregion
}

