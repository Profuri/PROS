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





    private int _round;
    private StageObject _currentStageObject;

    [SerializeField] protected List<BaseMapEvent> _mapEventList;
    
    [SerializeField] protected float _minRandomEvnetTime = 10f; 
    [SerializeField] protected float _maxRandomEvnetTime = 30f;

    protected Coroutine _generateCor;

    public virtual void Init(int mapIndex)
    {
        _mapEventList = new List<BaseMapEvent>();
        transform.Find("MapEvents").GetComponents(_mapEventList);
        _mapEventList.ForEach(mapEvent => mapEvent.Init(StageManager.Instance.MapBound));

        _round = 1;
        ItemManager.Instance.StartGenerateItem();
        ScoreManager.Instance.OnDecideWinnerEvent += OnDecideWinner;
        GenerateNewStage(mapIndex);
    }

    public virtual void StageLeave()
    {
        ItemManager.Instance.StopGenerateItem();
        ScoreManager.Instance.OnDecideWinnerEvent -= OnDecideWinner;
        RemoveCurStage();
    }

    public virtual void StageUpdate()
    {
        if (!NetworkManager.Instance.IsMasterClient || !PlayerManager.Instance.IsAllOfPlayerLoad)
            return;
        
        if (RoundCheck(out var roundWinner))
        {
            if (roundWinner == null)
            {
                return;
            }
            
            ++_round;
            Scoring(roundWinner);
            
            StageManager.Instance.RemoveCurMap();
            StageManager.Instance.GenerateNewMap();
        }
    }

    public virtual void OnDecideWinner(Player winner)
    {
        Debug.Log(winner.NickName);
    }

    public virtual void Scoring(Player targetPlayer)
    {
        ScoreManager.Instance.AddScore(targetPlayer);
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
        
        PlayerManager.Instance.RoundStart();

            if (_generateCor != null)
            {
                StopCoroutine(_generateCor);
            }
            _generateCor = StartCoroutine(GenerateMapEvent());
    }

    public virtual void RemoveCurStage()
    {
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
                Debug.Log("GeenrateMapEvent");
                timer = 0f;
                randomTime = Random.Range(_minRandomEvnetTime, _maxRandomEvnetTime);
                GetRandomBaseMapEvent()?.StartEvent();
            }
            timer += Time.deltaTime;
            
            yield return null;
        }
    }

    protected BaseMapEvent GetRandomBaseMapEvent()
    {
        int index = Random.Range(0,_mapEventList.Count);
        return _mapEventList[index];
    }
    public abstract bool RoundCheck(out Player roundWinner);
}