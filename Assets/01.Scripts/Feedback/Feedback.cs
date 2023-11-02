using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Feedback : MonoBehaviour
{
    protected Transform _agentTransform;
    public abstract void CreateFeedback();
    public abstract void FinishFeedback();

    public virtual void Init(Transform agent)
    {
        _agentTransform = agent;
    }

    protected virtual void OnDisable()
    {
        FinishFeedback();
    }
}
