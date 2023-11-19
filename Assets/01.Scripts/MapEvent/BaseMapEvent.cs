using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMapEvent : MonoBehaviour
{
    protected MapBoundStruct _mapBound;

    public virtual void Init(MapBoundStruct mapBound)
    {
        _mapBound = mapBound;
    }
    public abstract void EndEvent();
    public abstract void StartEvent();
    public abstract void ExecuteEvent();
}
