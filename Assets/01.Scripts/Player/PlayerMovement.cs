using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(Collider2D),typeof(Rigidbody2D))]
public class PlayerMovement : PlayerHandler
{
    private Rigidbody2D _rigidbody;
    [SerializeField] private Collider2D _collider;
    private Vector3 _inputVec3;

    public bool IsGrounded => Physics2D.BoxCast(transform.position,
        _collider.bounds.size,0,Vector3.down,0.1f, 1 << LayerMask.NameToLayer("GROUND"));
    
    public override void Init(PlayerBrain brain)
    {
        base.Init(brain);
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        
        _brain.InputSO.OnJumpKeyPress += Jump;
        _brain.InputSO.OnMovementKeyPress += Movement;
    }
    private void Movement(Vector2 value)
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
        Debug.Log($"IsGrounded: {IsGrounded}");
        transform.position += _inputVec3 * (_brain.MovementSO.Speed * Time.fixedDeltaTime);
    }
}
