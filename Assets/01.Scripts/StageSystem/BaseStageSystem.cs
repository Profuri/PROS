using System;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public abstract class BaseStageSystem : MonoBehaviour, IStageSystem
{
    [SerializeField] private EStageMode _mode;
    public EStageMode Mode => _mode;

    private int _round;

    public virtual void Init()
    {
        _round = 1;
        ScoreManager.Instance.OnDecideWinnerEvent += OnDecideWinner;
        GenerateNewStage();
    }

    public virtual void StageLeave()
    {
        ScoreManager.Instance.OnDecideWinnerEvent -= OnDecideWinner;
        RemoveCurStage();
    }

    public virtual void StageUpdate()
    {
        if (RoundCheck(out var roundWinner))
        {
            if (roundWinner == null)
            {
                return;
            }
            
            ++_round;
            Scoring(roundWinner);
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

    public abstract bool RoundCheck(out Player roundWinner);
    public abstract void GenerateNewStage();
    public abstract void RemoveCurStage();
}