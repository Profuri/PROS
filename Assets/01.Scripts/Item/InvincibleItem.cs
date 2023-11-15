using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class InvincibleItem : BaseItem
{
    [SerializeField] private float _shakeDuration = 0.5f;
    [SerializeField] private float _detectDistance = 5f;
    [SerializeField] private float _turnAngleValue = 100f;
    [SerializeField] private float _stopMoveDuration = 2f;
    [SerializeField] private float _basePower = 5f;
    [SerializeField] private UnityEvent RunAwayEvt;
    private float _stopTime = 0f;
    private bool _isturn = false;
    private bool _isMove = false;
    private Vector2 _targetMovePos;
    
    Camera _mainCam;
    Collider2D[] playerCol;
    public override void Init()
    {
        if (_mainCam == null)
            _mainCam = Camera.main;
        //_runawayTime = 0;
        //_moveDir = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized;
        //transform.position = new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
        //var moveSpeed = Random.Range(1f, 2f);
        //_movementSpeed = moveSpeed;
        //HitEvent.AddListener(ShakePosition);
        playerCol = Physics2D.OverlapCircleAll(transform.position, 100f, LayerMask.GetMask("DAMAGEABLE"));
    }
    public override void OnTakeItem(Player takenPlayer)
    {
        //PlayerManager.Instance.BrainDictionary[takenPlayer].GetComponent<PlayerBuff>().AddBuff(EBuffType.INVINCIBLE);
    }

    public override void GenerateSetting(Vector2 moveDir, Vector2 spawnPos, float movementSpeed)
    {
        base.GenerateSetting(moveDir, spawnPos, movementSpeed);
        playerCol = Physics2D.OverlapCircleAll(transform.position, 100f, LayerMask.GetMask("DAMAGEABLE"));
        _stopTime = 0f;
        _isturn = false;
        _isMove = false;
    }

    private void Start()
    {
        Init();
    }


    private void Update()
    {

        QuidditchMove();

        if (_isMove)
            transform.position = Vector2.Lerp(transform.position, _targetMovePos, Time.deltaTime * _basePower);

    }

    public override void UpdateItem()
    {
        if (Used || !_isSpawnEnd)
        {
            return;
        }

        QuidditchMove();
        if (_isMove)
            transform.position = Vector2.Lerp(transform.position, _targetMovePos, Time.deltaTime * _basePower);
    }

    void QuidditchMove()
    {
        #region 근처 플레이어 찾기.

        int nearColindex = 0; // min Distance collider index;
        float curDis = 0f;
        float minDis = float.MaxValue;
        
        for (int i = 0; i < playerCol.Length; i++)
        {
            curDis = Vector3.Distance(playerCol[i].transform.position, transform.position);
            if (curDis < minDis)
            {
                nearColindex = i;
                minDis = curDis;
            }
        }

        #endregion


        Vector3 centerpos = transform.position;
        Vector2 targetPos = 
            -(playerCol[nearColindex].transform.position - centerpos).normalized * (_basePower * Random.Range(0.5f, 1.5f));
        Vector2 viewportPoint = _mainCam.WorldToViewportPoint(targetPos);
        if (viewportPoint.x <= 0 || viewportPoint.x >= 1)
        {
            targetPos.x = -targetPos.x;
        }
        if (viewportPoint.y <= 0 || viewportPoint.y >= 1)// 카메라 범위 벗어난것
        {
            targetPos.y = -targetPos.y;
        }

        if (minDis < _detectDistance && _stopTime > _stopMoveDuration) // 멈춰있던 시간 끝
        {
            _isMove = true;
            //dir = 
        }
        else
        {
            _canMoveTime = 0;
            
            _moveDir = ;

            _canMoveTime += Time.deltaTime;
        }
    }

    public void ShakePosition()
    {
        Tween shakeTween = transform.DOShakePosition(_shakeDuration);
        shakeTween.Play();
    }

}