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

        foreach (EGAME_MODE gameMode in Enum.GetValues(typeof(EGAME_MODE)))
        {
            try
            {
                var methodInfo = this.GetType().GetMethod($"DoGameMode{gameMode.ToString()}",
                    BindingFlags.Instance | BindingFlags.NonPublic);
            }
            catch
            {
                Debug.LogError($"Can't Find this method from this game mode {gameMode}");
            }
        }


        NetworkManager.Instance.Init();
        SceneManagement.Instance.Init(this.transform);
        PlayerManager.Instance.Init();
        ParticleManager.Instance.Init();
        ScoreManager.Instance.Init();
        PoolManager.Instance = new PoolManager(this.transform);

        _poolingListSO.pairs.ForEach(p => PoolManager.Instance.CreatePool(p.prefab,p.count));
    }

    public void OTCPlayer(Player player, Vector3 attackDir)
    {
        var playerBrain = PlayerManager.Instance.BrainDictionary[player];
        playerBrain.PlayerOTC.Damaged(attackDir);

        if (playerBrain.PlayerDefend.IsDefendBounce)
        {
            playerBrain.PlayerDefend.IsDefendBounce = false;
            return;
        }

        switch (CurrentGameMode)
        {
            case EGAME_MODE.DEATH_MATCH:
                PlayerManager.Instance.RoundRestart();
                break;
            case EGAME_MODE.AREA_SEIZE:
                // Todo : Player가 다시 소환 되어야 함
                break;
        }
    }
}

