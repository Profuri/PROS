using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
public class PlayerMovement : PlayerHandler
{
    private Vector3 _inputVec3;
    public Vector3 InputVec => _inputVec3;
    
    private Coroutine _stopCoroutine;
    private float _originGravityScale;

    public bool IsStopped { get; set; }
    public bool IsGrounded => Physics2D.BoxCast(_brain.AgentTrm.position,
        _brain.Collider.bounds.size,0,Vector3.down,0.5f, 1 << LayerMask.NameToLayer("GROUND"));

    private float _jumpingTime = 0f;
    public override void Init(PlayerBrain brain)
    {
        base.Init(brain);

        Debug.Log(_brain.AnimationController);
        //_brain.InputSO.OnJumpKeyPress += _brain.AnimationController.PlayJumpAnim;
        _brain.InputSO.OnJumpKeyPress += SetJumping;

        _brain.InputSO.OnMovementKeyPress += SetInputVec;
        _originGravityScale = _brain.Rigidbody.gravityScale;
        _jumpingTime = 0f;
        StopAllCoroutines();
    }
    private void SetInputVec(Vector2 value) => _inputVec3 = value;

    private void SetJumping(Vector2 value) => _brain.ActionData.IsJumping = true; 
    public void JumpAction()
    {
        //if (!IsGrounded || !_brain.IsMine) return;
        Debug.Log("Jump");
        _brain.Rigidbody.velocity += new Vector2(0,_brain.MovementSO.JumpPower);
    }
    public override void BrainUpdate()
    {
        //If not dashing rotate to origin rotation
        if(_brain.ActionData.IsJumping) _jumpingTime += Time.deltaTime;
       
        if (_brain.ActionData.IsDashing == false)
        {
            if (transform.rotation != Quaternion.identity)
            {
                float percent = Mathf.Abs(Quaternion.identity.z - transform.rotation.z);
                SetRotationByDirection(Vector3.up,percent);
            }
        }

        if(IsGrounded)
        {
            if(_brain.ActionData.IsJumping == true && _jumpingTime >= 0.5f)
            {
                _brain.ActionData.IsLanding = true;
                _brain.AnimationController.PlayLandAnim(_brain.InputSO.CurrentInputValue);
                _brain.ActionData.IsJumping = false;
                _jumpingTime = 0f;
            }
        }

    }
    public override void BrainFixedUpdate()
    {
        if (IsStopped || !_brain.IsMine) return;
        Vector2 movement = _inputVec3;
        movement.y = 0f;

        //if(_brain.ActionData.IsJumping == false && _brain.ActionData.IsLanding == false)
        //    _brain.AnimationController.PlayMoveAnim(movement);

        var actionData = _brain.ActionData;
        actionData.PreviousPos = _brain.AgentTrm.position;
        transform.position +=  (Vector3)( movement) * (_brain.MovementSO.Speed * Time.fixedDeltaTime);
        actionData.CurrentPos = _brain.AgentTrm.position;
        //Debug.Log("MoveDirection" + (transform.position - _brain.ActionData.PreviousPos).normalized);
    }
    
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.collider.gameObject.layer == LayerMask.NameToLayer("WALL"))
        {
            bool isLeft = transform.position.x - other.collider.transform.position.x > 0;
            
            if (isLeft && _inputVec3.x < 0) _brain.PhotonView.RPC("StopYSystem",RpcTarget.All);
            else if (!isLeft && _inputVec3.x > 0) _brain.PhotonView.RPC("StopYSystem", RpcTarget.All);
            else _brain.PhotonView.RPC("ResumeGravity", RpcTarget.All);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.collider.gameObject.layer == LayerMask.NameToLayer("WALL"))
        {
            _brain.PhotonView.RPC("ResumeGravity", RpcTarget.All);
        }
    }

    [PunRPC]
    private void StopYSystem()
    {
        Vector2 currentVelocity = _brain.Rigidbody.velocity;
        currentVelocity.y = 0f;
        _brain.Rigidbody.velocity = currentVelocity;
        _brain.Rigidbody.gravityScale = 0f;
    }

    [PunRPC]
    private void ResumeGravity()
    {
        _brain.Rigidbody.gravityScale = _originGravityScale;
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
    //�ڷ�ƾ�� �����ִ��� bool ������ Ȯ���� �� �ְ� �ؾ���.
    private IEnumerator StopCoroutine(float stopTime,Action Callback = null)
    {  
        //���⼭ 0�� ���·� �����͹����� �Ǹ� �״�� 0���� 0���� �ٲ��ִ� ���� �ǹ���.
        _brain.Rigidbody.gravityScale = 0f;
        _brain.Rigidbody.velocity = Vector3.zero;
        IsStopped = true;
        yield return new WaitForSeconds(stopTime);
        _brain.Rigidbody.gravityScale = _originGravityScale;
        IsStopped = false;
        Callback?.Invoke();
    }
    #endregion
}
