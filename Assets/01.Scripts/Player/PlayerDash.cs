using System.Collections;
using Photon.Pun;
using UnityEngine;
using static Ease;
public class PlayerDash : PlayerHandler
{
    private Coroutine _dashCoroutine;
    [SerializeField] private float _dashTime = 0.15f;
    
    public bool CanDash => !_brain.PlayerMovement.IsGrounded && !_isDashed;
    private bool _isDashed = false;

    [SerializeField] private LayerMask _layer;
    public override void Init(PlayerBrain brain)
    {
        base.Init(brain);
        _brain.InputSO.OnDashKeyPress += DashRPC;
        _brain.OnDisableEvent += () => _brain.InputSO.OnDashKeyPress -= DashRPC;
        StopAllCoroutines();
    }
    #region Dash
    private void DashRPC()
    {
        if (_brain.IsMine)
        {
            Vector3 mouseDir = (_brain.MousePos - transform.position).normalized;
            _brain.PhotonView.RPC("Dash", RpcTarget.All,mouseDir);
        }
    }
        
    [PunRPC]
    private void Dash(Vector3 mouseDir)
    {
        if (CanDash)
        {
            if (_dashCoroutine != null)
            {
                StopCoroutine(_dashCoroutine);
            }
            float dashPower = _brain.MovementSO.DashPower;
            _isDashed = true;
            
            _dashCoroutine = StartCoroutine(DashCoroutine(_dashTime,dashPower,mouseDir));
        }
    }
        
    private IEnumerator DashCoroutine(float dashTime,float power,Vector3 mouseDir)
    {
        float timer = 0f;
        float prevValue = 0f;
        _brain.PlayerMovement.StopImmediately(dashTime);
        
        float radius = _brain.Collider.bounds.size.x * 0.5f;
        Vector3 destination = transform.position + mouseDir * power;
        
        _brain.ActionData.IsDashing = true;
        
        //목표 위치까지 현재 시간을 대쉬 타임만큼 나누어서 0 ~ 1로 만들어줌
        //그 위치마다 충돌체클르 해주고 로테이션을 돌려줌

        while (timer < dashTime)
        {
            timer += Time.deltaTime;
            float percent = Mathf.Lerp(0,1,timer / dashTime);
            float easingValue = EaseOutExpo(percent);
            float stepEasingValue = easingValue - prevValue;
            
            prevValue = easingValue;
            transform.position = Vector3.Lerp(transform.position,destination,stepEasingValue);

            
            _brain.PlayerMovement.SetRotationByDirection(mouseDir,easingValue);
            
            Collider2D collider = Physics2D.OverlapCircle(transform.position,radius,_layer);
            
            if (collider != default(Collider2D))
            {
                int curLayer = collider.gameObject.layer;
                if (curLayer == LayerMask.NameToLayer("DAMAGEABLE"))
                {
                    if (collider.TryGetComponent(out IDamageable damageable))
                    {
                        damageable.Damaged(this.transform,mouseDir,null);
                        transform.rotation = Quaternion.identity;
                        _brain.ActionData.IsDashing = false;
                        _brain.Rigidbody.velocity = Vector3.zero;
                        yield break;
                    }
                }
                else if (curLayer == LayerMask.NameToLayer("GROUND"))
                {
                    _brain.Rigidbody.velocity = Vector3.zero;
                }
                else if (curLayer == LayerMask.NameToLayer("WALL"))
                {
                    _brain.Rigidbody.velocity = Vector3.zero;
                }
            }
            yield return null;
        }
        
        //착륙 지점에 충돌체크를 한 번 더 해줌
        _brain.Rigidbody.velocity = Vector3.zero;
        _brain.ActionData.IsDashing = false;
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position,radius * 1.3f,_layer);
        if (cols.Length > 0)
        {
            foreach (var col in cols)
            {
                if (col.TryGetComponent(out IDamageable damageable))
                {
                    damageable.Damaged(this.transform,mouseDir,null);
                }
            }
        }

    }
    #endregion

    public override void BrainUpdate()
    {
        if (_brain.PlayerMovement.IsGrounded) _isDashed = false;
    }

    public override void BrainFixedUpdate()
    {
    }
}
