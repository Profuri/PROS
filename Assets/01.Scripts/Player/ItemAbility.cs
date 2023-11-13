using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class ItemAbility : PlayerHandler
{
    EBuffType _currentBuff = EBuffType.NONE;

    public void UseSkill(EBuffType buff)
    {
        _currentBuff |= buff;

        switch (_currentBuff)
        {
            case EBuffType.NONE:
                Debug.LogError("No buff added");
                break;
            case EBuffType.INVINCIBLE:
                Invincible();
                break;
            case EBuffType.DASHING:
                Dashing();
                break;
            case EBuffType.RANGEUP:
                RangeUp();
                break;
            case EBuffType.HEAVY:
                Heavy();
                break;
            case EBuffType.DOUBLEDASH:
                DoubleDash();
                break;
        }
    }
    
    public void RevertSkill(EBuffType buff)
    {
        _currentBuff &= ~buff;

        switch (buff)
        {
            case EBuffType.NONE:
                Debug.LogError("Buff not determined");
                break;
            case EBuffType.INVINCIBLE:
                RevertInvincible();
                break;
            case EBuffType.DASHING:
                RevertDashing();
                break;
            case EBuffType.RANGEUP:
                RevertRangeUp();
                break;
            case EBuffType.HEAVY:
                RevertHeavy();
                break;
            case EBuffType.DOUBLEDASH:
                RevertDoubleDash();
                break;
        }
    }

    #region Use_Skill_Function

    private void Invincible()
    {
    
    }
    
    private void Dashing()
    {
    
    }
    
    private void RangeUp()
    {
    
    }
    
    private void Heavy()
    {
    
    }
    
    private void DoubleDash()
    {
    
    }

    #endregion
    #region Revert_Skill_Function

    private void RevertInvincible()
    {

    }

    private void RevertDashing()
    {

    }

    private void RevertRangeUp()
    {

    }

    private void RevertHeavy()
    {

    }

    private void RevertDoubleDash()
    {

    }

    #endregion

    public override void BrainUpdate(){}
    public override void BrainFixedUpdate(){}
}
