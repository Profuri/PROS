using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMapEvent : MonoBehaviour
{
    public abstract void EndEvent();
    public abstract void StartEvent();
    public abstract void ExecuteEvent();
}
