using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class PlayerBuff : PlayerHandler // 상수 나중에 바꿀거야!
{
    private EBuffType _currentBuff = EBuffType.NONE;
    public EBuffType CurrentBuff => _currentBuff;

    public event Action<bool> Invincible;
    public event Action<bool> Dashing;
    public event Action<float, float> RangeUp;
    public event Action<float> Heavy;
    public event Action<bool, float> DoubleDash;

    [HideInInspector]
    public bool IsDashing;
    [HideInInspector]
    public bool IsDoubleDashing;

    public override void Init(PlayerBrain brain)
    {
        base.Init(brain);
        DisconnectEvent();
    }

    public void AddBuff(EBuffType buff)
    {
        _currentBuff |= buff;

        switch (_currentBuff)
        {
            case EBuffType.NONE:
                Debug.LogError("No Buff Added");
                break;
            case EBuffType.INVINCIBLE:
                Invincible?.Invoke(true);
                break;
            case EBuffType.DASHING:
                Dashing?.Invoke(true);
                break;
            case EBuffType.RANGEUP:
                RangeUp?.Invoke(2, 5f);
                break;
            case EBuffType.HEAVY:
                Heavy?.Invoke(50);
                break;
            case EBuffType.DOUBLEDASH:
                DoubleDash?.Invoke(true, 5f);
                break;
        }
    }
    
    public void RevertBuff(EBuffType buff)
    {
        _currentBuff &= ~buff;

        switch (buff)
        {
            case EBuffType.NONE:
                Debug.LogError("Buff Not Determined");
                break;
            case EBuffType.INVINCIBLE:
                Invincible?.Invoke(false);
                break;
            case EBuffType.DASHING:
                Dashing?.Invoke(false);
                break;
            case EBuffType.RANGEUP:
                RangeUp?.Invoke(1.5f, 5f);
                break;
            case EBuffType.HEAVY:
                Heavy?.Invoke(500);
                break;
            case EBuffType.DOUBLEDASH:
                DoubleDash?.Invoke(false, 5f);
                break;
        }
    }

    private void DisconnectEvent()
    {
        if (Invincible != null) Invincible = null;
        if (Dashing != null) Dashing = null;
        if (RangeUp != null) RangeUp = null;
        if (Heavy != null) Heavy = null;
        if (DoubleDash != null) DoubleDash = null;
    }

    private void OnDestroy()
    {
        DisconnectEvent();
    }

    public override void BrainUpdate(){}

    public override void BrainFixedUpdate(){}
}
