using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerHandler : MonoBehaviour
{
    protected PlayerBrain _brain;
    
    public virtual void Init(PlayerBrain brain)
    {
        this._brain = brain;
        
        _brain.OnUpdateEvent += BrainUpdate;
        _brain.OnFixedUpdateEvent += BrainFixedUpdate;
        _brain.OnEnableEvent += BrainEnable;
        _brain.OnDisableEvent += BrainDisable;
    }
    public abstract void BrainUpdate();
    public abstract void BrainFixedUpdate();
    public virtual void BrainEnable(){}
    public virtual void BrainDisable(){}
}
