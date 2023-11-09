using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccupationStageSystem : BaseStageSystem
{
    private OccupationSystem _occupationSystem;
    [SerializeField] private LayerMask _targetLayerMask;
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
        _occupationSystem.SetOccupationPos(Vector3.zero);
    }

    public override void RemoveCurStage()
    {
        
    }
    
}
