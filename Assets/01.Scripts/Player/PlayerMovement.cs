using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
public class PlayerMovement : PlayerHandler
{
    private Vector3 _inputVec3 = Vector3.zero;
    public Vector3 InputVec => _inputVec3;
    private Vector2 _prevInputVec = Vector2.zero;
    public Vector2 PrevInputVec => _prevInputVec;
    
    private Coroutine _stopCoroutine;
    private float _originGravityScale;

    public bool IsStopped { get; set; }
    
    public bool IsGrounded => Physics2D.BoxCast(_brain.AgentTrm.position,
        _brain.Collider.bounds.size,0,Vector3.down,0.5f, 1 << LayerMask.NameToLayer("GROUND"));

    public Vector3 GroundPos => Physics2D.BoxCast(_brain.AgentTrm.position,
        _brain.Collider.bounds.size,0,Vector3.down,0.5f, 1 << LayerMask.NameToLayer("GROUND")).point;

    public Vector3 GroundNormal => Physics2D.BoxCast(_brain.AgentTrm.position,
        _brain.Collider.bounds.size,0,Vector3.down,0.5f, 1 << LayerMask.NameToLayer("GROUND")).normal;
    
    public bool CanJump { get; private set; }
    private float _jumpingTime = 0f;
    public bool CanMoveAnim { get; private set; } = false;

    public override void Init(PlayerBrain brain)
    {
        base.Init(brain);

        Debug.Log(_brain.AnimationController);
        _brain.InputSO.OnJumpKeyPress += SetCanJump;
        _brain.InputSO.OnMovementKeyPress += SetInputVec;
        _originGravityScale = _brain.Rigidbody.gravityScale;
        _jumpingTime = 0f;
        StopAllCoroutines();
    }
    private void SetCanJump(Vector2 value) => CanJump = true;
    private void SetInputVec(Vector2 value) => _inputVec3 = value;
    public void PlayMoveAnim(Vector2 _input)
    {
        if(_brain.IsMine)
            photonView.RPC(nameof(PlayMoveAnimRPC), RpcTarget.All, _input);
    }
    public void PlayJumpAnim(Vector2 _input)
    {
        if (_brain.IsMine)
        {
            _brain.ActionData.IsJumping = true;
            photonView.RPC(nameof(PlayJumpAnimRPC), RpcTarget.All, _input);
        }
    }
    public void PlayLandAnim(Vector2 _input)
    {
        if(_brain.IsMine)
        {
            photonView.RPC(nameof(PlayLandAnimRPC), RpcTarget.All, _input);
            _brain.ActionData.IsLanding = true;
            _brain.ActionData.IsJumping = false;
            _jumpingTime = 0f;
        }
    }
    
    [PunRPC]
    public void PlayJumpAnimRPC(Vector2 _input)
    {
        _brain.AnimationController.PlayJumpAnim(_input);
    }

    [PunRPC]
    public void PlayMoveAnimRPC(Vector2 _input)
    {
        _brain.AnimationController.PlayMoveAnim(_input);
    }

    [PunRPC]
    public void PlayLandAnimRPC(Vector2 _input)
    {
        _brain.AnimationController.PlayLandAnim(_input);
        _prevInputVec = Vector2.zero;
    }    

    public void JumpAction() //Play Action in JumpAnim Event
    {
        //if (!IsGrounded || !_brain.IsMine) return;
        _brain.Rigidbody.velocity += new Vector2(0,_brain.MovementSO.JumpPower);
    }

    public override void BrainUpdate()
    {
        if (_brain.IsMine == false) return;
        //If not dashing rotate to origin rotation
        if(_brain.ActionData.IsJumping) _jumpingTime += Time.deltaTime;

        Vector2 movement = _inputVec3;
        if (_brain.ActionData.IsJumping == false)
        {
            //if (_prevInputVec != movement) //?�력??바꼇?�때�??�행?�줌
            if(_brain.ActionData.IsLanding == false)
                PlayMoveAnim(movement);
        }

        //if(_brain.ActionData.IsLanding == false)
        //{

        //}

        if (IsGrounded)
        {
            if(CanJump)
            {
                PlayJumpAnim(movement);
                CanJump = false;
            }

            if (_brain.ActionData.IsJumping == true && _jumpingTime >= 0.5f)
            {
                PlayLandAnim(movement);
            }
        }

        if (_brain.ActionData.IsDashing == false)
        {
            if (transform.rotation != Quaternion.identity)
            {
                float percent = Mathf.Abs(Quaternion.identity.z - transform.rotation.z);
                SetRotationByDirection(Vector3.up,percent);
            }
        }
        else
        {
            _brain.ActionData.IsJumping = false;
            _jumpingTime = 0f;
        }

      
        _prevInputVec = _inputVec3; // ?�전 ?�력 갱신. 
    }

    public override void BrainFixedUpdate()
    {
        if (IsStopped || !_brain.IsMine) return;
        Vector2 movement = _inputVec3;
        movement.y = 0f;

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
