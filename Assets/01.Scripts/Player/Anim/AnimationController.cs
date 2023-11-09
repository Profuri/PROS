using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D.IK;

[Flags]
public enum EKeyInputState 
{ 
    Left, Right, Idle, Down, Up
}
public class AnimationController : MonoBehaviour
{
    #region Hash
    private readonly int _MoveLHash = Animator.StringToHash("MoveLeft");
    private readonly int _MoveRHash = Animator.StringToHash("MoveRight");
    private readonly int _MoveIdleHash = Animator.StringToHash("MoveIdle");
    private readonly int _LandLHash = Animator.StringToHash("LandLeft");
    private readonly int _LandRHash = Animator.StringToHash("LandRight");
    private readonly int _LandIdleHash = Animator.StringToHash("LandIdle");
    private readonly int _jumpLHash = Animator.StringToHash("JumpLeft");
    private readonly int _jumpRHash = Animator.StringToHash("JumpRight");
    private readonly int _jumpIdleHash = Animator.StringToHash("JumpIdle");
    private readonly int _dashHash = Animator.StringToHash("Dash");
    private readonly int _sitDownHash = Animator.StringToHash("SitDown");
    private readonly int _sitUpHash = Animator.StringToHash("SitUp");

    #endregion
    #region Event

    public UnityEvent LandEvent;
    public UnityEvent JumpEvent;
    public UnityEvent DashEvent;
    public void PlayJumpEvent() => JumpEvent?.Invoke();
    public void PlayLandEvent() => LandEvent?.Invoke();
    public void PlayDashEvent() => DashEvent?.Invoke();
    
    #endregion
    
    //[SerializeField] Transform _playerTrm;
    //[SerializeField] LimbSolver2D _leftSolver;
    //[SerializeField] LimbSolver2D _rightSolver;
    //[SerializeField] private float m_Speed;
    //[SerializeField] private LayerMask _groundLayer;

    //public float DashTime = 1f;
    //public float JumpPower = 10f;
    //public float StepWeight = 1f;
    //public float StepSpeed = 0.2f;
    //public float FloatingValue = 0.5f; //발 얼마나 띄울지
    //public float PositionRadius = 1f; // check ground radius

    //private Rigidbody2D _rb;

    //private EKeyInputState _inputState = EKeyInputState.Idle;
    //private bool _isRight = true;
    //private bool _isUp = true;
    //private bool _prevDirIsRight = true;
    //private bool _onGround = false;
    //private bool _stepDone = false;

    //private bool _jumping = false;
    //private bool _landing = false;
    //private bool _dashing = false;
    
    //private float _jumpingTime = 0f;
    //public float AfterJumpCheckGroundTimeLimit = 0.2f;

    private Animator _animator;
    private PlayerBrain _brain;
    private void Awake()
    {
        //_leftlegTrm = _leftSolver.transform;
        //_rightlegTrm = _rightSolver.transform;
        _animator = GetComponent<Animator>();
        _brain = GetComponent<PlayerBrain>();
        //_rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        LandEvent.AddListener(() => _brain.ActionData.IsLanding = false);
    }

    //private void Start()
    //{
    //    LandEvent.AddListener(() => _landing = false);    
    //    DashEvent.AddListener(() => _dashing = false);    
    //}

    //public void Update()
    //{
    //    if (_jumping) _jumpingTime += Time.deltaTime;

    //    float x = Input.GetAxisRaw("Horizontal");
    //    if (x != 0)
    //    {
    //        _isRight = x > 0;
    //        //FlipIk(_isRight);
    //        if (_isRight)
    //        {
    //            _inputState = EKeyInputState.Right;
    //            //StartCoroutine(WalkRight());
    //        }
    //        else
    //        {
    //            _inputState = EKeyInputState.Left;
    //            //StartCoroutine(WalkLeft());
    //        }
    //        _playerTrm.position += new Vector3(x, 0, 0) * m_Speed * Time.deltaTime;
    //    }
    //    else
    //    {
    //        _inputState = EKeyInputState.Idle;
    //        SetPrevDirIsRight(_isRight);
    //    }

    //    //if(Input.GetKey(KeyCode.S))
    //    //{
    //    //    SitDown(true);
    //    //}
    //    //else if(Input.GetKeyUp(KeyCode.S) && _inputState == EKeyInputState.Down)
    //    //{
    //    //    SitDown(false);
    //    //}

    //    if(_jumping == true && _jumpingTime >= AfterJumpCheckGroundTimeLimit) //점프한지 0.2초가 흘ㄹ렀다면
    //    {
    //        if(GroundCheck())
    //        {
    //            _landing = true;
    //            _jumping = false;
    //            PlayLandAnim(_inputState);
    //        }
    //    }

    //    if (_landing == true || _dashing == true) return;
    //    if (_jumping == false && Input.GetKeyDown(KeyCode.Space))
    //    {
    //        PlayJumpAnim(_inputState);
    //        _jumping = true;
    //        _jumpingTime = 0f;
    //    }

    //    if (_jumping == false)
    //    {
    //        PlayMoveAnim(_inputState);
    //    }

    //    if(Input.GetKeyDown(KeyCode.LeftShift))
    //    {
    //        _dashing = true;
    //        PlayDashAnim();
    //    }
    //}

    //public bool GroundCheck() =>
    //    Physics2D.OverlapCircle(_playerTrm.position, PositionRadius, _groundLayer);
    //public void Jump()
    //{
    //    _rb.AddForce(Vector2.up * JumpPower, ForceMode2D.Impulse);
    //}


    //public void Dash()
    //{
    //    Vector3 mouseDir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
    //    mouseDir.z = 0;
    //    StartCoroutine(DashCoroutine(30, mouseDir));
    //}
    //private IEnumerator DashCoroutine(float power, Vector3 mouseDir)
    //{
    //    float prevValue = 0f;
    //    float timer = 0f;

    //    Debug.LogError($"MousePOs: {mouseDir}");
    //    Vector3 destination = transform.position + mouseDir * power;
    //    Debug.LogError($"Destination {destination}");
    //    float distanceFromDestination = Vector3.Distance(transform.position, destination);

    //    float timeToArrive = distanceFromDestination / power * DashTime;

    //    float percent = 0f;
    //    _rb.gravityScale = 0;

    //    //목표 위치까지 현재 시간을 대쉬 타임만큼 나누어서 0 ~ 1로 만들어줌
    //    //그 위치마다 충돌체클르 해주고 로테이션을 돌려줌
    //    //제대로된 PLAYER의 Brain과 Player를 찾아옴
    //    while (timer < timeToArrive)
    //    {
    //        timer += Time.deltaTime;

    //        percent = timer / timeToArrive;

    //        float easingValue = Ease.EaseOutCubic(percent);
    //        float stepEasingValue = easingValue - prevValue;

    //        prevValue = easingValue;

    //        var pos = Vector3.Lerp(transform.position, destination, stepEasingValue);
    //        transform.position = pos;


    //       SetRotationByDirection(mouseDir, easingValue);


    //        //CheckCollisionRealtime
    //        //현재 플레이어가 움직이면서 부딪히는 것을 확인하는 코드
    //        yield return null;
    //    }
    //    _rb.gravityScale = 9.8f;
    //}

    //#region RotationSystem
    //public void SetRotationByDirection(Vector3 direction)
    //{
    //    float desireAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
    //    transform.rotation = Quaternion.AngleAxis(desireAngle, Vector3.forward);
    //}
    //public void SetRotationByDirection(Vector3 direction, float lerpValue)
    //{
    //    float desireAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
    //    Quaternion targetRot = Quaternion.AngleAxis(desireAngle, Vector3.forward);
    //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, lerpValue);
    //}
    //#endregion
    #region PlayAnim
    public void SitDown(bool value)
    {
        if (value) _animator.Play(_sitDownHash); 
        else _animator.Play(_sitUpHash); 
    }
    public void PlayDashAnim()
    {
        _animator.Play(_dashHash);
    }
    
    public void PlayLandAnim(Vector2 _input)
    {
        EKeyInputState inputstate;
        if (_input.x != 0)
            inputstate = _input.x < 0 ? EKeyInputState.Left : EKeyInputState.Right;
        else
            inputstate = EKeyInputState.Idle;

        switch (inputstate)
        {
            case EKeyInputState.Left:
                _animator.Play(_LandLHash);
                break;
            case EKeyInputState.Right:
                _animator.Play(_LandRHash);
                break;
            case EKeyInputState.Idle:
                _animator.Play(_LandIdleHash);
                break;
        }
        Debug.Log("Land");
    }
    
    public void PlayMoveAnim(Vector2 _input)
    {
        EKeyInputState inputstate;
        if (_input.x != 0)
            inputstate = _input.x < 0 ? EKeyInputState.Left : EKeyInputState.Right;
        else
            inputstate = EKeyInputState.Idle;

        switch (inputstate)
        {
            case EKeyInputState.Left:
                _animator.Play(_MoveLHash);
                break;
            case EKeyInputState.Right:
                _animator.Play(_MoveRHash);
                break;
            case EKeyInputState.Idle:
                _animator.Play(_MoveIdleHash);
                break;
        }
        Debug.Log("Move");
    }

    public void PlayJumpAnim(Vector2 _input)
    {
        EKeyInputState inputstate;
        if (_input.x != 0)
            inputstate = _input.x < 0 ? EKeyInputState.Left : EKeyInputState.Right;
        else
            inputstate = EKeyInputState.Idle;

        switch(inputstate)
        {
            case EKeyInputState.Left:
                _animator.Play(_jumpLHash);
                break;
            case EKeyInputState.Right:
                _animator.Play(_jumpRHash);
                break;
            case EKeyInputState.Idle:
                _animator.Play(_jumpIdleHash);
                break;
        }
        Debug.Log("Jump");
    }
    #endregion
    #region Animwithcode
    //private Vector2 NextStepPos()
    //{
    //    Vector2 dir = _isRight ?
    //      Vector2.down + Vector2.right * StepWeight :
    //      Vector2.down + Vector2.left * StepWeight;
    //    RaycastHit2D hit = Physics2D.Raycast(_playerTrm.position, dir);
    //    return hit.collider != null ? hit.point : Vector2.zero;
    //}


    //public void SetPrevDirIsRight(bool value)
    //{
    //    if (_prevDirIsRight != value) _prevDirIsRight = value;
    //}
    #endregion
    //public void FlipIk(bool isRight)
    //{
    //    _leftSolver.flip = isRight == false ? true : false;
    //    _rightSolver.flip = isRight == false ? true : false;
    //}

    //private IEnumerator WalkLeft()
    //{

    //}

    //private IEnumerator WalkRight()
    //{

    //}

}
