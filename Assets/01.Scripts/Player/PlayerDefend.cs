using Photon.Pun;
using System.Collections;
using UnityEngine;

public class PlayerDefend : PlayerHandler
{
    [SerializeField] private float _coolTime = 2f;
    [SerializeField] private float _defendTime = 1.5f;

    private Coroutine _defendCoroutine;
    private float _prevTime;

    private bool _isDefend;
    public bool IsDefend => _isDefend;

    public bool IsDefendBounce = false;

    public override void Init(PlayerBrain brain)
    {
        base.Init(brain);
        _prevTime = Time.time;
        _brain.InputSO.OnDefendKeyPress += DefendRPC;
        _brain.OnDisableEvent += () => _brain.InputSO.OnDefendKeyPress -= DefendRPC;

        _brain.PlayerBuff.Invincible += Invincible;
        StopAllCoroutines();
    }
    
    private void DefendRPC()
    {
        if (_brain.IsMine)
        {
            _brain.PhotonView.RPC("Defend", RpcTarget.All);
        }
    }

    [PunRPC]
    private void Defend()
    {
        if (_prevTime + _coolTime >= Time.time) return;
        if (_defendCoroutine != null)
        {
            StopCoroutine(_defendCoroutine);
        }

        _isDefend = true;
        _prevTime = Time.time;
        _defendCoroutine = StartCoroutine(DefendCoroutine());
    }

    private void Invincible(bool value)
    {
        if (_brain.IsMine)
            _brain.PlayerDefend._isDefend = value;
    }

    IEnumerator DefendCoroutine()
    {        
        yield return new WaitForSeconds(_defendTime);
        _isDefend = false;
    }

    public override void BrainFixedUpdate(){}
    public override void BrainUpdate(){}
}
