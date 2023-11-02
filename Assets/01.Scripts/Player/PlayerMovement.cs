using System;
using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : PlayerHandler
{
    private Vector3 _inputVec3;
    public Vector3 InputVec => _inputVec3;
    private Rigidbody2D _rigidbody;
    private Coroutine _stopCoroutine;
    public bool IsStopped { get; private set; }
    public bool IsGrounded => Physics2D.BoxCast(transform.position,
        _brain.Collider.bounds.size,0,Vector3.down,0.1f, 1 << LayerMask.NameToLayer("GROUND"));
        
    public override void Init(PlayerBrain brain)
    {
        base.Init(brain);

        _rigidbody = GetComponent<Rigidbody2D>();
        _brain.InputSO.OnJumpKeyPress += Jump;
        _brain.InputSO.OnMovementKeyPress += SetInputVec;
        StopAllCoroutines();
    }
    private void SetInputVec(Vector2 value) => _inputVec3 = value;
    private void Jump()
    {
        if (!IsGrounded || !_brain.IsMine) return;
        Debug.Log("Jump");
        _rigidbody.velocity += new Vector2(0,_brain.MovementSO.JumpPower);
    }
    public override void BrainUpdate()
    {
        //If not dashing rotate to origin rotation
        if (_brain.ActionData.IsDashing == false)
        {
            if (transform.rotation != Quaternion.identity)
            {
                float percent = Mathf.Abs(Quaternion.identity.z - transform.rotation.z);
                SetRotationByDirection(Vector3.up,percent);
            }
        }
    }
    public override void BrainFixedUpdate()
    {
        if (IsStopped || !_brain.IsMine) return;
        Vector2 movement = _inputVec3;
        movement.y = 0f;
        
        transform.position +=  (Vector3)( movement) * (_brain.MovementSO.Speed * Time.fixedDeltaTime);
    }
    
    #region RotationSystem
    public void SetRotationByDirection(Vector3 direction)
    {
        float desireAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.AngleAxis(desireAngle,Vector3.forward);
    }
    public void SetRotationByDirection(Vector3 direction, float lerpValue)
    {
        float desireAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRot = Quaternion.AngleAxis(desireAngle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation,targetRot,lerpValue);
    }
    #endregion
    
    #region StopSystem
    public void StopImmediately(float stopTime, Action Callback = null)
    {
        if (_stopCoroutine != null)
        {
            StopCoroutine(_stopCoroutine);
        }
        _stopCoroutine = StartCoroutine(StopCoroutine(stopTime, Callback));
    }
    //코루틴이 멈춰있는지 bool 값으로 확인할 수 있게 해야함
    private IEnumerator StopCoroutine(float stopTime,Action Callback = null)
    {
        float originGravityMultiplier = _rigidbody.gravityScale;
        _rigidbody.gravityScale = 0f;
        _rigidbody.velocity = Vector3.zero;
        IsStopped = true;
        yield return new WaitForSeconds(stopTime);
        _rigidbody.gravityScale = originGravityMultiplier;
        IsStopped = false;
        Callback?.Invoke();
    }
    #endregion
}
