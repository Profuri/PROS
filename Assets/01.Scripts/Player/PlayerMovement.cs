using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using static Define;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : PlayerHandler
{
    private Vector3 _inputVec3;
    private Rigidbody2D _rigidbody;
    public Vector3 InputVec => _inputVec3;
    
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
    private void SetInputVec(Vector2 value)
    {
        _inputVec3 = value;
    }
    
    private void Jump()
    {
        //|| !_brain.IsMine
        if (!IsGrounded ) return;   
        Debug.Log("Jump");
        _rigidbody.velocity += new Vector2(0,_brain.MovementSO.JumpPower);
    }
    
    public override void BrainUpdate()
    {
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
        // || !_brain.IsMine
        if (IsStopped) return;
        Vector2 movement = _inputVec3;
        movement.y = 0f;
        
        transform.position +=  (Vector3)( movement) * (_brain.MovementSO.Speed * Time.fixedDeltaTime);
    }

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
    
    public void StopImmediately(float stopTime,Action Callback = null) => _stopCoroutine = StartCoroutine(StopCoroutine(stopTime,Callback));
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
}
