using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RangeUpItem : BaseItem
{
    [SerializeField] private float _shakeDuration = 0.5f;
    [SerializeField] private float _detectRadius = 5f;
    [SerializeField] private float _turnAngleValue = 100f;
    [SerializeField] private float _runawayMaxtime = 2f;
    [SerializeField] private float _minSpeed = 2f;
    [SerializeField] private float _maxSpeed = 4f;
    [SerializeField] private float _rangeAreaDivide = 6;
    private float _runawayTime;
    private bool _isturn = false;
    Camera _mainCam;

    public override void Init()
    {
        if(_mainCam == null)
            _mainCam = Camera.main;
        //_runawayTime = 0;
        //_moveDir = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized;
        //transform.position = new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
        //var moveSpeed = Random.Range(1f, 2f);
        //_movementSpeed = moveSpeed;
        ////HitEvent.AddListener(ShakePosition);
    }
    public override void OnTakeItem(Player takenPlayer)
    {
        //PlayerManager.Instance.BrainDictionary[takenPlayer].GetComponent<PlayerBuff>().AddBuff(EBuffType.INVINCIBLE);
    }

    public override void GenerateSetting(Vector2 moveDir, Vector2 spawnPos, float movementSpeed)
    {
        base.GenerateSetting(moveDir, spawnPos, movementSpeed);
        _runawayTime = 0f;
        _isturn = false;
    }

    //private void Start()
    //{
    //    Init();
    //}


    //private void Update()
    //{

    //    RunAwayMove();
    //    transform.Translate(_moveDir * (_movementSpeed * Time.deltaTime), Space.World);

    //}

    public override void UpdateItem()
    {
        base.UpdateItem();
        RunAwayMove();
        transform.Translate(_moveDir * (_movementSpeed * Time.deltaTime), Space.World);
    }

    void RunAwayMove()
    {
        #region ��ó �÷��̾� ã��.
        Collider2D[] playerCol = Physics2D.OverlapCircleAll(transform.position, _detectRadius, LayerMask.GetMask("DAMAGEABLE"));

        int nearColindex = 0; // min Distance collider index;
        float curDis = 0f;
        float minDis = float.MaxValue;
        //float maxDis = float.MinValue;
        for (int i = 0; i < playerCol.Length; i++)
        {
            curDis = Vector3.Distance(playerCol[i].transform.position, transform.position);
            if (curDis < minDis)
            {
                nearColindex = i;
                minDis = curDis;
            }
            //if(curDis > maxDis)
            //{
            //    maxDis = curDis;
            //}
        }

        #endregion

        Vector3 dir;
        if (playerCol.Length != 0)
        {
            _runawayTime = 0;
            dir = -(playerCol[nearColindex].transform.position - transform.position).normalized;
        }
        else
        {
            if (_runawayTime > _runawayMaxtime)
            {
                int x = Screen.width / 2;
                int y = Screen.height / 2;
                Vector2 screenCenter = 
                    new Vector2(x + Random.Range(-x/ _rangeAreaDivide, x/ _rangeAreaDivide),
                               y + Random.Range(-y/ _rangeAreaDivide, y / _rangeAreaDivide));

                _moveDir = (_mainCam.ScreenToWorldPoint(screenCenter) - transform.position).normalized; //�߽��� ���� ������    
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

}
