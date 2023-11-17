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
    [SerializeField] private float _baseStopMoveDuration = 1.3f;
    [SerializeField] private float _stoptimeDeviation = 1f; // 시간 편차
    [SerializeField] private float _basePower = 5f;
    [SerializeField] private float _powerDeviation = 1f; // 힘 편차
    [SerializeField] private UnityEvent RunAwayEvt;
    private float _stopTime = 0f;
    private float _stopMoveDuration;
    
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
        _stopMoveDuration = 
            _baseStopMoveDuration + Random.Range(-_stoptimeDeviation / 2, +_stoptimeDeviation / 2);
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
            -(playerCol[nearColindex].transform.position - centerpos).normalized 
            * (_basePower + Random.Range(-_powerDeviation/2, +_powerDeviation));
        Vector2 viewportPoint = _mainCam.WorldToViewportPoint(targetPos);
        if (viewportPoint.x <= 0 || viewportPoint.x >= 1)
        {
            targetPos.x = -targetPos.x;
        }
        if (viewportPoint.y <= 0 || viewportPoint.y >= 1)// 카메라 범위 벗어난것.
        {
            targetPos.y = -targetPos.y;
        }

        if (minDis < _detectDistance && _stopTime > _stopMoveDuration) // 멈춰있던 시간 끝.
        {
            if(!_isMove)
            {
                _stopMoveDuration =
                    _baseStopMoveDuration + Random.Range(-_stoptimeDeviation / 2, +_stoptimeDeviation / 2); //다음 정지 시간을 미리 계산.
            }
            _isMove = true;
            _targetMovePos = targetPos;
            _stopTime = 0;
        }
        else
        {
            if(Vector3.Distance(_targetMovePos, transform.position) < 0.1f)
            {
                _isMove = false;
                _targetMovePos = transform.position;
            }
            if(!_isMove)
                _stopTime += Time.deltaTime;
            //_targetMovePos = transform.position;
        }
    }

    public void ShakePosition()
    {
        Tween shakeTween = transform.DOShakePosition(_shakeDuration);
        shakeTween.Play();
    }

}