using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class InvincibleItem : BaseItem
{
    [SerializeField] private float _shakeDuration = 0.5f;
    [SerializeField] private float _detectRadius = 5f;
    [SerializeField] private float turnAngleValue = 100f;
    public override void Init()
    {
        HitEvent.AddListener(ShakePosition);
    }

    public override void OnTakeItem(Player takenPlayer)
    {
        
    }

    public override void UpdateItem()
    {
        base.UpdateItem();
        
        RunAwayMove();
    }

    void RunAwayMove()
    {
        #region 근처 플레이어 찾기.
        Collider2D[] playerCol = Physics2D.OverlapCircleAll(transform.position, _detectRadius);
        
        int index = 0; // min Distance collider index;
        float curDis = 0f;
        float minDis = float.MaxValue;

        for(int i = 0; i < playerCol.Length; i++)
        {
            curDis = Vector3.Distance(playerCol[i].transform.position, transform.position);
            if (curDis < minDis)
            {
                index = i;
                minDis = curDis;
            }
        }
        #endregion

        Vector3 dir = -(playerCol[index].transform.position - transform.position).normalized;

        float dot = Vector3.Dot(transform.up, dir);
        if (dot < 1.0f)
        {
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
            Vector3 cross = Vector3.Cross(transform.up, dir);
            if (cross.z < 0)
            {
                angle = transform.rotation.eulerAngles.z - Mathf.Min(turnAngleValue * Time.deltaTime, angle);
            }
            else
            {
                angle = transform.rotation.eulerAngles.z + Mathf.Min(turnAngleValue * Time.deltaTime, angle);
            }
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        _moveDir = transform.up;
    }

    public void ShakePosition()
    {
        Tween shakeTween = transform.DOShakePosition(_shakeDuration);
        shakeTween.Play();
    }

    private void OnDisable()
    {
        HitEvent.RemoveAllListeners();
    }
}
