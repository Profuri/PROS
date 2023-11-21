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

    public event Action<bool, float> Invincible;
    public event Action<bool, float> Dashing;
    public event Action<float, float> RangeUp;
    public event Action<float, float> Heavy;
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
                Invincible?.Invoke(true, 10f);
                break;
            case EBuffType.DASHING:
                Dashing?.Invoke(true, 10f);
                break;
            case EBuffType.RANGEUP:
                RangeUp?.Invoke(2, 10f);
                break;
            case EBuffType.HEAVY:
                Heavy?.Invoke(50, 10f);
                break;
            case EBuffType.DOUBLEDASH:
                DoubleDash?.Invoke(true, 10f);
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
                Invincible?.Invoke(false, 1f);
                break;
            case EBuffType.DASHING:
                Dashing?.Invoke(false, 1f);
                break;
            case EBuffType.RANGEUP:
                RangeUp?.Invoke(1.5f, 1f);
                break;
            case EBuffType.HEAVY:
                Heavy?.Invoke(500, 1f);
                break;
            case EBuffType.DOUBLEDASH:
                DoubleDash?.Invoke(false, 1f);
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
