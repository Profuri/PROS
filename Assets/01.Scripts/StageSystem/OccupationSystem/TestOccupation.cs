using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOccupation : MonoBehaviour
{
    private BaseStageSystem _baseStageSystem;

    private void Awake()
    {
        _baseStageSystem = GetComponent<BaseStageSystem>();
        _baseStageSystem.Init(EStageMode.OCCUPATION);
        
        
    }
}
