using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider2D),typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputSO _inputSO;
    [SerializeField] private MovementSO _movementSO;
    
    private Rigidbody _rigidbody;
    private Collider _collider;
    private Vector3 _inputVec3;

    public bool IsGrounded => Physics2D.BoxCast(transform.position,
        _collider.bounds.size,0,Vector3.down,0.2f);
    
    public void Init()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        
        _inputSO.OnJumpKeyPress += Jump;
        _inputSO.OnMovementKeyPress += Movement;
    }

    private void FixedUpdate()
    {
        Debug.Log($"IsGrounded: {IsGrounded}");
        transform.position += _inputVec3 * (_movementSO.Speed * Time.fixedDeltaTime);
    }
    private void Movement(Vector2 value)
    {
        _inputVec3 = value;
    }
    private void Jump()
    {
        if (!IsGrounded) return;   
        _rigidbody.velocity += new Vector3(0,_movementSO.JumpPower,0);
    }
    private void Dash()
    {
        
    }
}
