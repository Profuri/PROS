using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : PlayerHandler
{
    private Vector3 _inputVec3;
    public Vector3 InputVec => _inputVec3;
    
    private Coroutine _stopCoroutine;

    public bool IsStopped { get; private set; }
    public bool IsGrounded => Physics2D.BoxCast(transform.position,
        _brain.Collider.bounds.size,0,Vector3.down,0.1f, 1 << LayerMask.NameToLayer("GROUND"));
                                                                                            
    public override void Init(PlayerBrain brain)
    {
        base.Init(brain);

        _brain.InputSO.OnJumpKeyPress += Jump;
        _brain.InputSO.OnMovementKeyPress += SetInputVec;
        StopAllCoroutines();
    }
    private void SetInputVec(Vector2 value) => _inputVec3 = value;
    private void Jump()
    {
        if (!IsGrounded || !_brain.IsMine) return;
        Debug.Log("Jump");
        _brain.Rigidbody.velocity += new Vector2(0,_brain.MovementSO.JumpPower);
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

        var actionData = _brain.ActionData;
        actionData.PreviousPos = transform.position;
        transform.position +=  (Vector3)( movement) * (_brain.MovementSO.Speed * Time.fixedDeltaTime);
        actionData.CurrentPos = transform.position;
        Debug.Log(actionData.PreviousPos +actionData.CurrentPos);

        //Debug.Log("MoveDirection" + (transform.position - _brain.ActionData.PreviousPos).normalized);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.gameObject.layer == LayerMask.NameToLayer("WALL"))
        {
            bool isLeft = transform.position.x - other.collider.transform.position.x > 0;

            if (isLeft && _inputVec3.x < 0)
            {
                _brain.Rigidbody.gravityScale = 0f;
            }
            else if(!isLeft && _inputVec3.x > 0)
            {
                _brain.Rigidbody.gravityScale = 0f;
            }
        }
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
        float originGravityMultiplier = _brain.Rigidbody.gravityScale;
        _brain.Rigidbody.gravityScale = 0f;
        _brain.Rigidbody.velocity = Vector3.zero;
        IsStopped = true;
        yield return new WaitForSeconds(stopTime);
        _brain.Rigidbody.gravityScale = originGravityMultiplier;
        IsStopped = false;
        Callback?.Invoke();
    }
    #endregion
}
