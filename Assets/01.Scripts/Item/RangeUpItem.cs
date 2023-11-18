using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RangeUpItem : BaseItem
{
    [SerializeField] private float _shakeDuration = 0.5f;
    [SerializeField] private float _detectDistance = 5f;
    [SerializeField] private float _turnAngleValue = 100f;
    [SerializeField] private float _runawayMaxtime = 2f;
    [SerializeField] private float _runawayDelaytime = 2f;
    [SerializeField] private float _minSpeed = 2f;
    [SerializeField] private float _maxSpeed = 4f;
    private float _runawayTime;
    private bool _isturn = false;

    Collider2D[] playerCol;
    public override void Init()
    {

        //playerCol = Physics2D.OverlapCircleAll(transform.position, 100f, LayerMask.GetMask("DAMAGEABLE"));
        //HitEvent.AddListener(ShakePosition);
    }
    public override void OnTakeItem(Player takenPlayer)
    {
        //PlayerManager.Instance.BrainDictionary[takenPlayer].GetComponent<PlayerBuff>().AddBuff(EBuffType.INVINCIBLE);
    }

    public override void GenerateSetting(Vector2 moveDir, Vector2 spawnPos, float movementSpeed)
    {
        base.GenerateSetting(moveDir, spawnPos, movementSpeed);
        playerCol = Physics2D.OverlapCircleAll(transform.position, 100f, LayerMask.GetMask("DAMAGEABLE"));
        _runawayTime = 0f;
        _isturn = false;
    }


    //private void Update()
    //{

    //    RunAwayMove();
    //    transform.Translate(_moveDir * (_movementSpeed * Time.deltaTime), Space.World);

    //}

    public override void UpdateItem()
    {
        base.UpdateItem();
        RunAwayMove();
    }

    void RunAwayMove()
    {
        #region ��ó �÷��̾� ã��.
        //Collider2D[] playerCol = Physics2D.OverlapCircleAll(transform.position, _detectRadius, LayerMask.GetMask("DAMAGEABLE"));

        int index = 0; // min Distance collider index;
        float curDis = 0f;
        float minDis = float.MaxValue;

        for (int i = 0; i < playerCol.Length; i++)
        {
            curDis = Vector3.Distance(playerCol[i].transform.position, transform.position);
            if (curDis < minDis)
            {
                index = i;
                minDis = curDis;
            }
        }

        #endregion

        Vector3 dir;
        //if (playerCol.Length != 0)
        if (minDis < _detectDistance)
        {
            _runawayTime = 0;
            dir = -(playerCol[index].transform.position - transform.position).normalized;
        }
        else
        {
            if (_runawayTime > _runawayMaxtime)
            {
                //Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
                //_moveDir = (Camera.main.ScreenToWorldPoint(screenCenter) - transform.position).normalized; //�߽��� ���� ������    
                //for (int i = 0; i < playerCol.Length; i++)
                //{
                //    curDis = Vector3.Distance(playerCol[i].transform.position, transform.position);
                //    if (curDis < minDis)
                //    {
                //        index = i;
                //        minDis = curDis;
                //    }
                //}
                _moveDir = (playerCol[index].transform.position - transform.position).normalized;
            }
            dir = _moveDir;

            _runawayTime += Time.deltaTime;
        }

        float dot = Vector3.Dot(transform.up, dir);
        if (dot < 1.0f)
        {
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
            Vector3 cross = Vector3.Cross(transform.up, dir);
            if (cross.z < 0)
            {
                angle = transform.rotation.eulerAngles.z - Mathf.Min(_turnAngleValue * Time.deltaTime, angle);
            }
            else
            {
                angle = transform.rotation.eulerAngles.z + Mathf.Min(_turnAngleValue * Time.deltaTime, angle);
            }
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        _moveDir = transform.up;
        float sinValue = Mathf.Sin(Time.time) / 2 + 0.5f;
        if (sinValue < 0.3)
            _movementSpeed = Mathf.Lerp(_minSpeed, _maxSpeed, sinValue);
        else if (sinValue < 0.8)
            _movementSpeed = Mathf.Lerp(_minSpeed, _maxSpeed, sinValue *= 3);
        else
            _movementSpeed = Mathf.Lerp(_minSpeed, _maxSpeed, sinValue);

    }

    public void ShakePosition()
    {
        Tween shakeTween = transform.DOShakePosition(_shakeDuration);
        shakeTween.Play();
    }

    private void OnDisable()
    {
        playerCol = null;
    }

}