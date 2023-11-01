using System.Collections;
using System.Data.Common;
using UnityEngine;
using static Ease;
public class PlayerDash : PlayerHandler
{
    private Coroutine _dashCoroutine;
    [SerializeField] private float _dashTime = 0.15f;

    public bool CanDash => !_brain.PlayerMovement.IsGrounded && !_isDashed;
    private bool _isDashed = false; 
    public override void Init(PlayerBrain brain)
    {
        base.Init(brain);
        _brain.InputSO.OnDashKeyPress += Dash;
        StopAllCoroutines();
    }

    private void Dash()
    {
        if (CanDash)
        {
            if (_dashCoroutine != null)
            {
                StopCoroutine(_dashCoroutine);
            }
            float dashPower = _brain.MovementSO.DashPower;
            _isDashed = true;
            
            _dashCoroutine = StartCoroutine(DashCoroutine(_dashTime,dashPower));
        }
    }
    
    private IEnumerator DashCoroutine(float dashTime,float power)
    {
        float timer = 0f;
        float prevValue = 0f;
        _brain.PlayerMovement.StopImmediately(dashTime);
        
        Vector3 mouseDir = (Camera.main.ScreenToWorldPoint((_brain.MousePos) - transform.position)).normalized;
        float radius = _brain.Collider.bounds.size.x * 0.5f;
        Vector3 destination = transform.position + mouseDir * power;
        
        var layer = 1 << LayerMask.NameToLayer("DAMAGEABLE");
        
        
        _brain.PlayerMovement.SetRotationByDirection(mouseDir);
        //timer 말고 Stop 되어있는 만큼 이동하는 방식으로 바꾸는게 더 나아보임
        while (timer < dashTime)
        {
            timer += Time.deltaTime;
            float percent = Mathf.Lerp(0,1,timer / dashTime);
            float easingValue = EaseOutExpo(percent);
            float stepEasingValue = easingValue - prevValue;
            
            prevValue = easingValue;

            transform.position = Vector3.Lerp(transform.position,destination,stepEasingValue);
            
            Collider2D collider = Physics2D.OverlapCircle(transform.position,radius,layer);
            
            if (collider != null)
            {
                if (collider.TryGetComponent(out IDamageable damageable))
                {
                    damageable.Damaged(transform.position,mouseDir);
                    yield break;
                }
            }
            yield return null;
        }

        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position,radius * 1.3f,layer);
        if (cols.Length > 0)
        {
            foreach (var col in cols)
            {
                if (col.TryGetComponent(out IDamageable damageable))
                {
                    Vector3 direction = mouseDir;
                    damageable.Damaged(transform.position,direction);
                }
            }
        }
        
        transform.rotation = Quaternion.identity;
    }

    public override void BrainUpdate()
    {
        if (_brain.PlayerMovement.IsGrounded) _isDashed = false;
    }

    public override void BrainFixedUpdate()
    {
    }
}
