using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : PlayerHandler
{
    private Rigidbody2D _rigidbody;
    private Vector3 _inputVec3;
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
        if (!IsGrounded) return;   
        _rigidbody.velocity += new Vector2(0,_brain.MovementSO.JumpPower);
    }
    public override void BrainUpdate()
    {
        
    }

    public override void BrainFixedUpdate()
    {
        if (IsStopped) return;
        Vector3 movement = _inputVec3;
        movement.y = Mathf.Min(0,_inputVec3.y);
        
        transform.position += movement * (_brain.MovementSO.Speed * Time.fixedDeltaTime);
    }

    public void SetRotationByDirection(Vector3 direction)
    {
        float desireAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.AngleAxis(desireAngle,Vector3.forward);
    }
    //코루틴이 멈춰있는지 bool 값으로 확인할 수 있게 해야함
    public void StopImmediately(float stopTime,Action Callback = null) => _stopCoroutine = StartCoroutine(StopCoroutine(stopTime,Callback));
    private IEnumerator StopCoroutine(float stopTime,Action Callback)
    {
        float originGravityScale = _rigidbody.gravityScale;
        _rigidbody.gravityScale = 0f;
        IsStopped = true;
        yield return new WaitForSeconds(stopTime);
        _rigidbody.gravityScale = originGravityScale;
        IsStopped = false;
        Callback?.Invoke();
    }
}
