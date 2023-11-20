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

    protected int _round;
    private StageObject _currentStageObject;

    [SerializeField] protected List<BaseMapEvent> _mapEventList;
    
    [SerializeField] protected float _minRandomEvnetTime = 10f; 
    [SerializeField] protected float _maxRandomEvnetTime = 30f;

    protected Coroutine _generateCor;

    protected bool _runningStage;

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

    public virtual void StageLeave()
    {
        ItemManager.Instance.StopGenerateItem();
        RemoveCurStage();
    }

    public virtual void StageUpdate()
    {
        if (!_runningStage || !NetworkManager.Instance.IsMasterClient || !PlayerManager.Instance.IsAllOfPlayerLoad)
        {
            return;
        }
        
        if (RoundCheck(out var roundWinner))
        {
            if (roundWinner == null)
            {
                return;
            }

            _runningStage = false;
            ++_round;
            
            ScoreManager.Instance.AddScore(roundWinner);
            StageManager.Instance.RoundWinner(roundWinner);
        }
    }

    public Vector3 GetRandomSpawnPoint()
    {
        if (!_currentStageObject)
        {
            Debug.LogError("Stage doesnt loaded");
            return Vector3.zero;
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
        if (!NetworkManager.Instance.IsMasterClient) return;

        if (_currentStageObject)
        {
            return;
        }

        _currentStageObject = PhotonNetwork.Instantiate($"Map{index}", Vector2.zero, Quaternion.identity).GetComponent<StageObject>();
        _currentStageObject?.Setting();

        Debug.LogError($"CurrentStageObject: {_currentStageObject}");
        
        PlayerManager.Instance.RoundStart();

        if (_generateCor != null)
        {
            StopCoroutine(_generateCor);
        }
        _runningStage = true;
        _generateCor = StartCoroutine(GenerateMapEvent());
    }

    public virtual void RemoveCurStage()
    {
        if(NetworkManager.Instance.IsMasterClient == false) return;
        PhotonNetwork.Destroy(_currentStageObject.gameObject);
        _currentStageObject = null;
        PlayerManager.Instance.RoundEnd();
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
                GetRandomBaseMapEvent()?.StartEvent();
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
    public abstract bool RoundCheck(out Player roundWinner);
}