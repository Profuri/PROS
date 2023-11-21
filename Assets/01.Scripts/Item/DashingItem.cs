using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using MonoPlayer;

public class DashingItem : BaseItem
{
    [SerializeField] private float _shakeDuration = 0.5f;
    [SerializeField] private float _detectDistance = 5f;
    [SerializeField] private float _baseStopMoveDuration = 1.3f;
    [SerializeField] private float _stoptimeDeviation = 1f; // �ð� ����
    [SerializeField] private float _basePower = 5f;
    [SerializeField] private float _powerDeviation = 1f; // �� ����
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
        //playerCol = Physics2D.OverlapCircleAll(transform.position, 100f, LayerMask.GetMask("DAMAGEABLE"));
        _stopTime = 0f;
        _stopMoveDuration =
            _baseStopMoveDuration + Random.Range(-_stoptimeDeviation / 2, +_stoptimeDeviation / 2);
        _isturn = false;
        _isMove = false;
    }

    public override void OnTakeItem(Player takenPlayer)
    {
        PlayerManager.Instance.BrainDictionary[takenPlayer].GetComponent<PlayerBuff>().AddBuff(EBuffType.DASHING);
    }

    public override void GenerateSetting(Vector2 moveDir, Vector2 spawnPos, float movementSpeed)
    {
        base.GenerateSetting(moveDir, spawnPos, movementSpeed);
        //playerCol = Physics2D.OverlapCircleAll(transform.position, 100f, LayerMask.GetMask("DAMAGEABLE"));
        _stopTime = 0f;
        _stopMoveDuration = 
            _baseStopMoveDuration + Random.Range(-_stoptimeDeviation / 2, +_stoptimeDeviation / 2);
        _isturn = false;
        _isMove = false;
    }

    #region Debug�ڵ�

    #endregion

    public override void UpdateItem()
    {
        _spawnT += Time.deltaTime;
        if (_spawnT > 1f) _isSpawnEnd = true;
        if (!_isSpawnEnd)
        {
            return;
        }

        QuidditchMove();
        if (_isMove)
            transform.position = Vector2.Lerp(transform.position, _targetMovePos, Time.deltaTime * _basePower);
    }

    void QuidditchMove()
    {
        #region ��ó �÷��̾� ã��.
        Collider2D[] playerCol = Physics2D.OverlapCircleAll(transform.position, 50f, LayerMask.GetMask("DAMAGEABLE"));

        int nearColindex = 0; // min Distance collider index;
        float curDis = 0f;
        float minDis = float.MaxValue;

        if (playerCol.Length == 0) return;

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
        if (viewportPoint.y <= 0 || viewportPoint.y >= 1)// ī�޶� ���� �����.
        {
            targetPos.y = -targetPos.y;
        }

        if (minDis < _detectDistance && _stopTime > _stopMoveDuration) // �����ִ� �ð� ��.
        {
            if(!_isMove)
            {
                _stopMoveDuration =
                    _baseStopMoveDuration + Random.Range(-_stoptimeDeviation / 2, +_stoptimeDeviation / 2); //���� ���� �ð��� �̸� ���.
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