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
    private float _curveTime;
    private float _turnDuration;
    private float _afterturnDuration; //돌고 난후 easing적용할 기간.
    //private bool _isturn = false;
    //private bool _constantspeed = true;
    private float _baseMovementSpeed;
    private bool _isTurning;

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
        _baseMovementSpeed = _movementSpeed;
        _runawayTime = 0f;
        _turnDuration = 2f;
        _curveTime = 0f;
        _isTurning = false;

        playerCol = Physics2D.OverlapCircleAll(transform.position, 100f, LayerMask.GetMask("DAMAGEABLE"));
    }

    private void Start()
    {
        GenerateSetting(Vector2.left, new Vector2(Random.Range(-5f, 5f),Random.Range(-5f,5f)), 2f);
    }

    private void Update()
    {

        RunAwayMove();
        transform.Translate(_moveDir * (_movementSpeed * Time.deltaTime), Space.World);

    }

    public override void UpdateItem()
    {
        base.UpdateItem();
        RunAwayMove();
    }

    void RunAwayMove()
    {
        #region 근처 플레이어 찾기.
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
        
        if(_isTurning && minDis < _detectDistance)
        {
            _movementSpeed = _baseMovementSpeed;
            _isTurning = false;
            _turnDuration = 2f;
        }
        else
        {
            if (dot < 0.9 && !(minDis < _detectDistance)) // turning
            {
                _isTurning = true;
                _curveTime += Time.deltaTime;
                _afterturnDuration = 0f;
                _movementSpeed = _baseMovementSpeed * CosInOut(_curveTime / _turnDuration);
                //Debug.Log("CosValue: " + CosInOut(_curveTime));
            }
            else
            {
                if (_isTurning)
                {
                    _afterturnDuration += Time.deltaTime;
                    if (_afterturnDuration > 0.3f)
                    {
                        if (_curveTime != 0)
                            _turnDuration = _curveTime;
                        _curveTime = 0f;
                        _movementSpeed = _baseMovementSpeed;
                        _isTurning = false;
                    }
                    else
                    {
                        _curveTime += Time.deltaTime;
                        _movementSpeed = _baseMovementSpeed * CosInOut(_curveTime / _turnDuration);
                    }
                }
                else
                {
                    //Debug.Log("Constant");
                }
            }
        }
        _moveDir = transform.up;
        
        //_movementSpeed = _baseMovementSpeed * CosInOut(Time.time);

        //Debug.Log("MovementSpeed : " + _movementSpeed);

        //float sinValue = Mathf.Sin(Time.time) / 2 + 0.5f;
        //if (sinValue < 0.3)
        //    _movementSpeed = Mathf.Lerp(_minSpeed, _maxSpeed, sinValue);
        //else if (sinValue < 0.8)
        //    _movementSpeed = Mathf.Lerp(_minSpeed, _maxSpeed, sinValue *= 3);
        //else
        //    _movementSpeed = Mathf.Lerp(_minSpeed, _maxSpeed, sinValue);

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

    private float EaseInOut(float t)
    {
        return t < 0.5 ? 2 * t * t : -1 + (4 - 2 * t) * t;
    }

    private float CosInOut(float t)
    {
        return (Mathf.Cos(6.3f * t) + 2) / 3;
    }
}
