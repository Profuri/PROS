using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using static Ease;

public class PlayerDash : PlayerHandler
{
    private Coroutine _dashCoroutine;
    [SerializeField] private float _dashTime = 0.3f;
    
    public bool CanDash => !_brain.PlayerMovement.IsGrounded && !_isDashed;
    private bool _isDashed = false;

    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private LayerMask _dashCollisionMask;

    public override void Init(PlayerBrain brain)
    {
        base.Init(brain);
        _brain.InputSO.OnDashKeyPress += DashRPC;
        _brain.OnDisableEvent += () => _brain.InputSO.OnDashKeyPress -= DashRPC;
        _brain.OnOTC += () => enabled = false;
        StopAllCoroutines();
    }
    
    #region Dash
    private void DashRPC()
    {
        if (_brain.IsMine)
        {
            Vector3 mouseDir = (_brain.MousePos - _brain.AgentTrm.position).normalized;
            _brain.PhotonView.RPC("Dash", RpcTarget.All,mouseDir);
        }
    }
    
    [PunRPC]
    private void Dash(Vector3 mouseDir)
    {
        if (CanDash)
        {
            //_brain.AnimationController.PlayDashAnim();
            if (_dashCoroutine != null)
            {
                StopCoroutine(_dashCoroutine);
            }
            float dashPower = _brain.MovementSO.DashPower;
            _isDashed = true;
            
            _dashCoroutine = StartCoroutine(DashCoroutine(dashPower,mouseDir));
        }
    }
    
    private IEnumerator DashCoroutine(float power, Vector3 mouseDir)
    {
        float prevValue = 0f;
        float timer = 0f;
        
        Vector3 destination = _brain.AgentTrm.position + (mouseDir * power);

        Debug.Log($"Power: {power}");
        Debug.Log($"Destionation: {destination}");

        float distanceFromDestination = Vector3.Distance(_brain.AgentTrm.position, destination);
        
        float timeToArrive = _dashTime;

        float radius = ((CircleCollider2D)_brain.Collider).radius * 1.5f;
            
        _brain.ActionData.IsDashing = true;
        
        float percent = 0f;
        
        var hit = Physics2D.Raycast(_brain.AgentTrm.position, mouseDir,distanceFromDestination, _obstacleMask);
        
        if (hit.collider is not null)
        {
            var obstacleCollider = hit.collider;
            destination = hit.point - (Vector2)mouseDir * (_brain.Collider.bounds.size / 2);
            timeToArrive = Vector3.Distance(_brain.AgentTrm.position, destination) / power * _dashTime; 
        }

        _brain.PlayerMovement.StopImmediately(timeToArrive);
        
        while (timer < timeToArrive)
        {
            timer += Time.deltaTime;
            percent = timer / timeToArrive;
            
            float easingValue = EaseOutCubic(percent);
            float stepEasingValue = easingValue - prevValue;
            
            prevValue = easingValue;
            
            var pos = Vector3.Lerp(_brain.AgentTrm.position,destination,stepEasingValue);
            transform.position = pos;

            _brain.PlayerMovement.SetRotationByDirection(mouseDir, easingValue);
            
            if (CheckDashCollision(mouseDir, radius))
            {
                transform.rotation = Quaternion.identity;
                yield break;
            }

            yield return null;
        }
        
        //_brain.PlayerMovement.StopImmediately(0.0f);
        transform.rotation = Quaternion.identity;
        CheckDashCollision(mouseDir, radius * 1.5f);
        _brain.ActionData.IsDashing = false;
    }
    #endregion

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
                    _brain.PlayerOTC.Damaged(-dir);
                }
                else
                {
                    collisionBrain.PlayerOTC.Damaged(dir);
                }

                returnValue = true;
            }
            
            if (cols[i].TryGetComponent<BaseItem>(out var item))
            {
                if (item.HitByPlayer(_brain.PhotonView.Owner))
                {
                    continue;
                }

                collisionBrain.PlayerDefend.IsDefendBounce = true;
                _brain.PlayerOTC.Damaged(-dir);
            }
        }

        return returnValue;
    }

    public override void BrainUpdate()
    {
        if (_brain.PlayerMovement.IsGrounded)
        {
            //if (_isDashed) _brain.AnimationController.PlayLandAnim(_brain.InputSO.CurrentInputValue);
            _isDashed = false;
        }
    }

    public override void BrainFixedUpdate()
    {

    }
}
