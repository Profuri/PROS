using System.Collections;
using System.Collections.Generic;
using MonoPlayer;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public abstract class BaseStageSystem : MonoBehaviour, IStageSystem
{
    [SerializeField] private EStageMode _mode;
    public EStageMode Mode => _mode;

    protected BaseMapEvent _currentMapEvent;

    protected int _round;
    private StageObject _currentStageObject;
    public StageObject CurStageObject => _currentStageObject;

    [SerializeField] protected List<BaseMapEvent> _mapEventList;
    
    [SerializeField] protected float _minRandomEvnetTime = 10f; 
    [SerializeField] protected float _maxRandomEvnetTime = 30f;

    protected Coroutine _generateCor;

    protected bool _runningStage;
    public bool RunningStage => _runningStage;

    public virtual void Init(int mapIndex)
    {
        _mapEventList = new List<BaseMapEvent>();
        GetComponents(_mapEventList);
        _mapEventList.ForEach(mapEvent => mapEvent.Init(StageManager.Instance.MapBound));

        _round = 1;
        ItemManager.Instance.StartGenerateItem();
        GenerateNewStage(mapIndex);

        
        _runningStage = true;
    }

    protected void RoundWinner(Player winner)
    {
        if (!_runningStage || !PlayerManager.Instance.IsAllOfPlayerLoad)
        {
            return;
        }
        
        _runningStage = false;
        ++_round;

        if (!ScoreManager.Instance.AddScore(winner))
        {
            StageManager.Instance.RoundWinner(winner);
        }
    }

    public virtual void StageLeave()
    {
        ItemManager.Instance.StopGenerateItem();
        RemoveCurStage();
    }

    public virtual void StageUpdate()
    {
        
    }

    public Vector3 GetRandomSpawnPoint()
    {
        if (!_currentStageObject)
        {
            _currentStageObject = PoolManager.Instance.Pop($"Map{StageManager.Instance.MapIdx}") as StageObject;
            
            if(!_currentStageObject)
            {
                Debug.LogError("Stage doesnt loaded");
                return Vector3.zero;
            }
        }
        
        if (_currentStageObject.SpawnPoints.Count <= 0)
        {
            Debug.LogWarning("Stage spawn points is empty!");
            return Vector3.zero;
        }
        
        return _currentStageObject.SpawnPoints[Random.Range(0, _currentStageObject.SpawnPoints.Count)];
    }

    public virtual void GenerateNewStage(int index)
    {
        if (_currentStageObject)
        {
            return;
        }

        _currentMapEvent?.EndEvent();
        _currentStageObject = PoolManager.Instance.Pop($"Map{index}") as StageObject;
        _currentStageObject?.Setting();

        Debug.Log($"CurrentStageObject: {_currentStageObject}");
        
        PlayerManager.Instance.RoundStart();

        _runningStage = true;

        if (NetworkManager.Instance.IsMasterClient)
        {
            if (_generateCor != null)
            {
                StopCoroutine(_generateCor);
            }
            _generateCor = StartCoroutine(GenerateMapEvent());
        }

    }

    public virtual void RemoveCurStage()
    {
        PoolManager.Instance.Push(_currentStageObject);

        _currentMapEvent?.EndEvent();
        _currentMapEvent = null;

        PlayerManager.Instance.RoundEnd();
        _currentStageObject = null;
    }
    
    protected IEnumerator GenerateMapEvent()
    {
        float timer = 0f;
        float randomTime = Random.Range(_minRandomEvnetTime, _maxRandomEvnetTime);
        while (true)
        {
            if (timer > randomTime)
            {
                timer = 0f;
                randomTime = Random.Range(_minRandomEvnetTime, _maxRandomEvnetTime);
                _currentMapEvent = GetRandomBaseMapEvent();
                _currentMapEvent?.StartEvent();
            }
            timer += Time.deltaTime;
            
            yield return null;
        }
    }

    private BaseMapEvent GetRandomBaseMapEvent()
    {
        int index = Random.Range(0,_mapEventList.Count);
        //int index = 1;
        return _mapEventList[index];
    }
    
    public abstract void RoundCheck(Player player);
}