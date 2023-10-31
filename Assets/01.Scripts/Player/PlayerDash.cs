using System.Collections;
using System.Data.Common;
using UnityEngine;
using static Ease;
public class PlayerDash : PlayerHandler
{
    private Coroutine _dashCoroutine;
    [SerializeField] private float _dashTime = 0.15f;
    public override void Init(PlayerBrain brain)
    {
        base.Init(brain);
        _brain.InputSO.OnDashKeyPress += Dash;
        StopAllCoroutines();
    }

    private void Dash()
    {
        if (_dashCoroutine != null)
        {
            StopCoroutine(_dashCoroutine);
        }
        float dashPower = _brain.MovementSO.DashPower;
        _dashCoroutine = StartCoroutine(DashCoroutine(_dashTime,dashPower));
    }
    private IEnumerator DashCoroutine(float dashTime,float power)
    {
        float timer = 0f;
        _brain.PlayerMovement.StopImmediately(dashTime);
        //timer 말고 Stop 되어있는 만큼 이동하는 방식으로 바꾸는게 더 나아보임
        while (timer < dashTime)
        {
            timer += Time.deltaTime;
            
            Vector3 inputVec = _brain.PlayerMovement.InputVec;
            float percent = Mathf.Lerp(0,1,timer / dashTime);
            
            transform.position += inputVec * (power * Time.deltaTime * EaseInBack(percent));
            Vector3 colSize = _brain.Collider.bounds.size;
            
            var hit2D = Physics2D.BoxCast(transform.position,colSize,0f,inputVec,0f,1 << LayerMask.NameToLayer("DAMAGEABLE"));
            
            if (hit2D != default(RaycastHit2D))
            {
                if (hit2D.collider.TryGetComponent(out IDamageable damageable))
                {
                    damageable.Damaged(transform.position,inputVec);
                    yield break;
                }
            }
            yield return null;
        }
    }

    public override void BrainUpdate()
    {
        
    }

    public override void BrainFixedUpdate()
    {
    }
}
