using MonoPlayer;

using Photon.Realtime;
using UnityEngine;

public abstract class BaseStageSystem : IStageSystem
{
    private EStageMode _mode;
    public EStageMode Mode => _mode;

    private int _round;
    private StageObject _currentStageObject;

    public BaseStageSystem(EStageMode mode)
    {
        _mode = mode;
    }

    public virtual void Init(int mapIndex)
    {
        _round = 1;
        ScoreManager.Instance.OnDecideWinnerEvent += OnDecideWinner;
        GenerateNewStage(mapIndex);
    }

    public virtual void StageLeave()
    {
        ScoreManager.Instance.OnDecideWinnerEvent -= OnDecideWinner;
        RemoveCurStage();
    }

    public virtual void StageUpdate()
    {
        if (!NetworkManager.Instance.IsMasterClient)
            return;
        
        if (RoundCheck(out var roundWinner))
        {
            if (roundWinner == null)
            {
                return;
            }
            
            ++_round;
            Scoring(roundWinner);
            RemoveCurStage();
            
            var type = Random.Range(0, StageManager.Instance.MapCnt) + 1;
            GenerateNewStage(type);
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
            Debug.LogError("Stage spawn points is empty!");
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

        _currentStageObject = PoolManager.Instance.Pop($"Stage{index}") as StageObject;
        _currentStageObject?.Setting();
        
        PlayerManager.Instance.RoundStart();
    }

    public virtual void RemoveCurStage()
    {
        PoolManager.Instance.Push(_currentStageObject);
        _currentStageObject = null;
        PlayerManager.Instance.RoundEnd();
    }

    public abstract bool RoundCheck(out Player roundWinner);
}