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
    
    private EGAME_MODE _currentGameMode = EGAME_MODE.DEATH_MATCH;
    public EGAME_MODE CurrentGameMode => _currentGameMode;

    #region INTERNAL VARIABLE
    [SerializeField] private int _targetWinCnt = 3;
    
    [SerializeField]
    private PoolingListSO _poolingListSO;
    
    private Dictionary<EGAME_MODE, MethodInfo> _gameMethodDictionary;
    private Dictionary<Player, int> _playerGameDictionary;
    
    private Coroutine _gameCoroutine;
    private Score _score;
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
        _score = new Score(PlayerManager.Instance.LoadedPlayerList,_targetWinCnt);
        
        _playerGameDictionary.Clear();
        foreach (var player in PlayerManager.Instance.LoadedPlayerList)
        {
            if (_playerGameDictionary.ContainsKey(player)) continue;
            _playerGameDictionary.Add(player,(int)EPLAYER_STATE.NORMAL);
        }
        
        RoundStart();
    }
    public void RoundStart()
    {
        //플레이어의 상태 초기화
        //InvalidOperationException

        if (NetworkManager.Instance.IsMasterClient)
        {
            PlayerManager.Instance.SetBrainRandomPos();
        }
        foreach (var pair in _playerGameDictionary.ToList())
        {
            _playerGameDictionary[pair.Key] = (int)EPLAYER_STATE.NORMAL;
        }
        //var gameMode = GetRandomGameMode();
        //ChangeGameMode(gameMode);
        //Debug.Log(gameMode);
        if (_gameCoroutine != null)
        {
            StopCoroutine(_gameCoroutine);
        }
        StartCoroutine(GameCoroutine(EGAME_MODE.DEATH_MATCH));
    }
    #region GameSystem
    private IEnumerator GameCoroutine(EGAME_MODE eGameMode)
    {
        MethodInfo methodInfo = _gameMethodDictionary[eGameMode];
        Debug.LogError($"MethodInfo: {methodInfo}");
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
            ScorePlayer(winPlayer);
            if (_gameCoroutine != null)
            {
                StopCoroutine(_gameCoroutine);
            }

            StartCoroutine(Test(winPlayer));
            //OnRoundEnd?.Invoke(winPlayer);
        } 
    }

    private void DoGameModeAREA_SEIZE()
    {
        //만약 영역 안에 플레이거 들어오면 플레이어의 현재 점수를 계속 올려주어야함
                    
                    
                    
                    
                    
        foreach (var kvp in _playerGameDictionary)
        {
            if (kvp.Value >= (int)EPLAYER_STATE.WIN)
            {
                ScorePlayer(kvp.Key);
                //OnRoundEnd?.Invoke(kvp.Key);

                StartCoroutine(Test(kvp.Key));
            }
        }
    }

    private IEnumerator Test(Player player)
    {
        yield return new WaitForSeconds(5f);
        OnRoundEnd?.Invoke(player);
    }
    private void ScorePlayer(Player player)
    {
        _score.GetScore(player);
        
        
        //if appear winPlayer OnGameEnd Invoked 
        //not appear winPlayer OnRoundEnd Invoked
        var winPlayer = _score.IsGameEnd;
        if (winPlayer != default(Player))
        {
            OnGameEnd?.Invoke(winPlayer);
        }
        else
        {
            OnRoundEnd?.Invoke(player);
        }
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

