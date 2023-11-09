using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public abstract class BaseStageSystem : MonoBehaviour, IStageSystem
{
    private EStageMode _mode;
    public EStageMode Mode => _mode;

    private int _round;

    public virtual void Init(EStageMode mode)
    {
        _mode = mode;
        _round = 1;

        //ScoreManager.Instance.OnDecideWinnerEvent += OnDecideWinner;

        GenerateNewStage();
    }

    public virtual void StageLeave()
    {
        //ScoreManager.Instance.OnDecideWinnerEvent -= OnDecideWinner;
        RemoveCurStage();
    }

    public virtual void StageUpdate()
    {
        if (RoundCheck())
        {
            ++_round;
            RemoveCurStage();
            GenerateNewStage();
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

    public abstract bool RoundCheck();
    public abstract void GenerateNewStage();
    public abstract void RemoveCurStage();
}