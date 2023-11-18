using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using MonoPlayer;
using UnityEngine;
using Photon.Pun;
enum Dir
{
    LEFT = -1,
    NONE = 0,
    RIGHT = 1,
    END,
}
public class WindMapEvent : BaseMapEvent
{
    [SerializeField] private float _windPower = 20f;

    [SerializeField] private LayerMask _targetLayer;
    
    [Tooltip("바람이 몇 시간동안 불지")]
    [SerializeField] private float _minWindTime;
    [SerializeField] private float _maxWindTime;

    [SerializeField] private Vector3 _colPos;
    [SerializeField] private Bounds _colBounds;
    

    private Dir _curDir;
    private Coroutine _coroutine;
    public override void StartEvent()
    {
        ExecuteEvent();
    }
    public override void EndEvent()
    {
        
    }
    public override void ExecuteEvent()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        StartCoroutine(WindCoroutine());
    }

    private IEnumerator WindCoroutine()
    {
        Debug.LogError("WindCoroutine");
        float timer = 0f;
        float targetTime = Random.Range(_minWindTime, _maxWindTime);

        //_curDir = (Dir)Random.Range((int)Dir.LEFT, (int)Dir.END);
        _curDir = Dir.LEFT;
        
        while (timer <= targetTime)
        {
            timer += Time.deltaTime;
            NetworkManager.Instance.PhotonView.RPC(nameof(ApplyWindRPC),RpcTarget.All,(int)_curDir);
            yield return null;
        }
    }
    
    //It Works master client only
    [PunRPC]
    private void ApplyWindRPC(int dir)
    {
        PlayerBrain brain = PlayerManager.Instance.BrainDictionary[NetworkManager.Instance.LocalPlayer];
        brain.PlayerMovement.ApplyWind(_windPower,dir);
    }
}
