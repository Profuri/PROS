using System;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using static Ease;

public class PlayerDash : PlayerHandler
{
    private Coroutine _dashCoroutine;
    [SerializeField] private float _dashTime = 0.3f;
    private float _LandRange = 1.5f;

    public bool CanDash => !_brain.PlayerMovement.IsGrounded && !_isDashed;
    private bool _isDashed = false;
    private bool _isDoubleDash = false;

    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private LayerMask _dashCollisionMask;
    [SerializeField] private AudioClip _dashClip;
    [SerializeField] private TrailRenderer _dashTrail;

    public override void Init(PlayerBrain brain)
    {
        base.Init(brain);
        _brain.InputSO.OnDashKeyPress += DashRPC;
        _brain.OnDisableEvent += () => _brain.InputSO.OnDashKeyPress -= DashRPC;
        _brain.PlayerBuff.RangeUp += LandRangeUpRPC;
        _brain.PlayerBuff.Dashing += DashingBuff;
        _brain.PlayerBuff.DoubleDash += DoubleDashBuff;
        _brain.OnOTC += () => enabled = false;
        StopAllCoroutines();
    }
    
    private void DashRPC()
    {
        if (_brain.IsMine && (CanDash || !_isDoubleDash))
        {
            Vector3 mouseDir = (_brain.MousePos - _brain.AgentTrm.position).normalized;
            _brain.PhotonView.RPC("Dash", RpcTarget.All,mouseDir);
        }
    }
    
    [PunRPC]
    private void Dash(Vector3 mouseDir)
    {
        if (_dashCoroutine != null)
        {
            StopCoroutine(_dashCoroutine);
        }
        float dashPower = _brain.MovementSO.DashPower;

        if (_isDashed && _brain.IsMine)
            _isDoubleDash = true;
        
        _isDashed = true;
        
        _dashCoroutine = StartCoroutine(DashCoroutine(dashPower,mouseDir));
    }

    private IEnumerator DashCoroutine(float power, Vector3 mouseDir)
    {
        AudioManager.Instance.Play(_dashClip);

        float timer = 0f;
        
        Vector3 destination = _brain.AgentTrm.position + (mouseDir * power);

        float distanceFromDestination = Vector3.Distance(_brain.AgentTrm.position, destination);
        float timeToArrive = _dashTime;
        float radius = ((CircleCollider2D)_brain.Collider).radius * _LandRange;
            
        _brain.ActionData.IsDashing = true;
        float percent = 0f;
        
        var hit = Physics2D.Raycast(_brain.AgentTrm.position, mouseDir,distanceFromDestination, _obstacleMask);
        
        if (hit.collider is not null)
        {
            var obstacleCollider = hit.collider;
            destination = hit.point - (Vector2)mouseDir * (_brain.Collider.bounds.size / 2);
            timeToArrive = Vector3.Distance(_brain.AgentTrm.position, destination) / power * _dashTime; 
        }

        _brain.AnimationController.PlayDashAnim();


        _brain.PlayerMovement.StopImmediately(timeToArrive);

        _dashTrail.enabled = true;
        while (timer < timeToArrive)
        {
            timer += Time.deltaTime;
            percent = timer / timeToArrive;
            
            transform.position = Vector3.Lerp(transform.position,destination,percent);

            //_brain.PlayerMovement.SetRotationByDirection(mouseDir);
            
            if (CheckDashCollision(mouseDir, radius))
            {
                //_brain.PlayerMovement.SetRotationByDirection(Vector3.up);
                transform.rotation = Quaternion.identity;
                _brain.AnimationController.PlayLandAnim(_brain.PlayerMovement.InputVec);
                yield break;
            }

            yield return null;
        }
        _dashTrail.enabled = false;

        transform.rotation = Quaternion.identity;
        _brain.AnimationController.PlayLandAnim(_brain.PlayerMovement.InputVec);
        _brain.PlayerMovement.StopImmediately(0.0f);
        _brain.ActionData.IsDashing = false;
    }

    private bool CheckDashCollision(Vector3 dir, float radius, int maxCheckCount = 10)
    {
        var cols = new Collider2D[maxCheckCount];
        var size = Physics2D.OverlapCircleNonAlloc(_brain.AgentTrm.position, radius, cols, _dashCollisionMask);

        var returnValue = false;
        
        if (size <= 0)
        {
            return false;
        } 
        
        for (var i = 0; i < size; i++)
        {
            if (cols[i].Equals(_brain.Collider))
            {
                continue;
            }

            if (cols[i].TryGetComponent<PlayerBrain>(out var collisionBrain))
            {
                _brain.PlayerMovement.StopImmediately(0.0f);
                ParticleManager.Instance.PlayParticleAll("ExplosionParticle", _brain.AgentTrm.position);
                
                if (collisionBrain.PlayerDefend.IsDefend)
                {
                    collisionBrain.PlayerDefend.IsDefendBounce = true;
                    _brain.PlayerOTC.Damaged(collisionBrain.PhotonView.Owner,-dir);
                }
                else
                {
                    collisionBrain.PlayerOTC.Damaged(_brain.PhotonView.Owner,dir);
                }

                returnValue = true;
            }
            
            if (cols[i].TryGetComponent<BaseItem>(out var item))
            {
                if (item.HitByPlayer(_brain.PhotonView.Owner))
                {
                    continue;
                }

                //collisionBrain.PlayerDefend.IsDefendBounce = true;
                //_brain.PlayerOTC.Damaged(collisionBrain.PhotonView.Owner, -dir);
            }
        }

        return returnValue;
    }

    private void LandRangeUpRPC(float value, float time)
    {
        if (_brain.IsMine)
            _brain.PhotonView.RPC("LandRangeUp", RpcTarget.All, value, time);
    }

    [PunRPC]
    private void LandRangeUp(float value, float time)
    {
        StopCoroutine(LandRangeUpTime(value, time));
        StartCoroutine(LandRangeUpTime(value, time));
    }

    private IEnumerator LandRangeUpTime(float value, float time)
    {
        _LandRange = value;
        Debug.Log("Play LangeUp");
        yield return new WaitForSeconds(time);
        if (_brain.PlayerBuff.CurrentBuff.HasFlag(EBuffType.RANGEUP))
            _brain.PlayerBuff.RevertBuff(EBuffType.RANGEUP);
        Debug.Log("End LangeUp");
    }

    private void DashingBuff(bool value, float time)
    {
        if (_brain.IsMine)
        {
            StopCoroutine(DashingTime(value, time));
            StartCoroutine(DashingTime(value, time));
        }
    }

    private IEnumerator DashingTime(bool value, float time)
    {
        Debug.Log("Play Dashing");
        _brain.PlayerBuff.IsDashing = value;
        yield return new WaitForSeconds(time);
        Debug.Log("End Dashing");
        if (_brain.PlayerBuff.CurrentBuff.HasFlag(EBuffType.DASHING))
            _brain.PlayerBuff.RevertBuff(EBuffType.DASHING);
    }

    private void DoubleDashBuff(bool value, float time)
    {
        if (_brain.IsMine)
        {
            StopCoroutine(DoubleDashTime(value, time));
            StartCoroutine(DoubleDashTime(value, time));
        }
    }

    private IEnumerator DoubleDashTime(bool value, float time)
    {
        Debug.Log("Play DoubleDash");
        _brain.PlayerBuff.IsDoubleDashing = value;
        yield return new WaitForSeconds(time);
        Debug.Log("End DoubleDash");
        if (_brain.PlayerBuff.CurrentBuff.HasFlag(EBuffType.DOUBLEDASH))
            _brain.PlayerBuff.RevertBuff(EBuffType.DOUBLEDASH);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_brain.PlayerBuff.IsDashing)
        {
            if (collision.gameObject.TryGetComponent<PlayerBrain>(out PlayerBrain player))
            {
                Vector3 dir = player.gameObject.transform.position.normalized - _brain.transform.position.normalized;
                //player.PlayerOTC.Damaged(player., dir);
            }
        }
    }

    public override void BrainUpdate()
    {
        if (_brain.PlayerMovement.IsGrounded)
        {
            if (_isDashed) _brain.AnimationController.PlayLandAnim(_brain.InputSO.CurrentInputValue);
            _isDashed = false;
            if (_brain.PlayerBuff.IsDoubleDashing && _brain.IsMine)
                _isDoubleDash = false;
        }
    }

    public override void BrainFixedUpdate()
    {

    }
}
