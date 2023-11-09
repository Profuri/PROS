using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class OccupationStageSystem : BaseStageSystem
{
    private OccupationSystem _occupationSystem;
    [SerializeField] private LayerMask _targetLayerMask;

    public Action OnTargetChangeTime;
    public override void StageUpdate()
    {
        base.StageUpdate();
    }
    public override bool RoundCheck()
    {
        return true;
    }

    public override void GenerateNewStage()
    {
        OccupationStruct data = new OccupationStruct(targetOccupationTime: 10f,
                        minChangeTime: 20f,maxChangeTime: 40f,5f,_targetLayerMask);
        
        _occupationSystem = new OccupationSystem(this,data);
        SetRandomOccupationPos();

        OnTargetChangeTime += SetRandomOccupationPos;
    }

    public override void RemoveCurStage()
    {
        
    }

    private void SetRandomOccupationPos()
    {
        //It will be changed by fixed value
        Vector3 randomPos = new Vector3(UnityEngine.Random.Range(-5,5f),0,0);
        
        _occupationSystem.SetOccupationPos(randomPos);
    }
    
}
