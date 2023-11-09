using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Realtime;
public class OccupationStageSystem : BaseStageSystem
{
    private OccupationSystem _occupationSystem;
    [SerializeField] private LayerMask _targetLayerMask;

    public Action OnTargetChangeTime;

    public OccupationStageSystem(EStageMode mode) : base(mode)
    {
    }


    public override void StageUpdate()
    {
        base.StageUpdate();
    }

    public override bool RoundCheck(out Player winnerPlayer)
    {
        winnerPlayer = null;
        return true;
    }

    public override void GenerateNewStage(int index)
    {
        base.GenerateNewStage(index);
        OccupationStruct data = new OccupationStruct(targetOccupationTime: 10f,
                        minChangeTime: 20f,maxChangeTime: 40f,5f,_targetLayerMask);
        
        _occupationSystem = new OccupationSystem(this,data);
        SetRandomOccupationPos();

        OnTargetChangeTime += SetRandomOccupationPos;
    }

    private void SetRandomOccupationPos()
    {
        //It will be changed by fixed value
        Vector3 randomPos = new Vector3(UnityEngine.Random.Range(-5,5f),0,0);
        
        _occupationSystem.SetOccupationPos(randomPos);
    }
}
