using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D.IK;

public enum EKeyInputState 
{ 
    Left, Right, Idle
}
public class AnimationController : MonoBehaviour
{
    private readonly int _MoveLHash = Animator.StringToHash("MoveLeft");
    private readonly int _MoveRHash = Animator.StringToHash("MoveRight");
    private readonly int _MoveIdleHash = Animator.StringToHash("MoveIdle");
    private readonly int _LandLHash = Animator.StringToHash("LandLeft");
    private readonly int _LandRHash = Animator.StringToHash("LandRight");
    private readonly int _LandIdleHash = Animator.StringToHash("LandIdle");
    private readonly int _jumpLHash = Animator.StringToHash("JumpLeft");
    private readonly int _jumpRHash = Animator.StringToHash("JumpRight");
    private readonly int _jumpIdleHash = Animator.StringToHash("JumpIdle");

    public UnityEvent LandEvent;
    public UnityEvent JumpEvent;
    
    [SerializeField] Transform _playerTrm;
    [SerializeField] LimbSolver2D _leftSolver;
    [SerializeField] LimbSolver2D _rightSolver;
    [SerializeField] private float m_Speed;
    [SerializeField] private LayerMask _groundLayer;

    public float JumpPower = 10f;
    public float StepWeight = 1f;
    public float StepSpeed = 0.2f;
    public float FloatingValue = 0.5f; //발 얼마나 띄울지
    public float PositionRadius = 1f; // check ground radius

    private Transform _leftlegTrm;
    private Transform _rightlegTrm;
    private Rigidbody2D _rb;

    private EKeyInputState _inputState = EKeyInputState.Idle;
    private bool _isRight = true;
    private bool _prevDirIsRight = true;
    private bool _onGround = false;
    private bool _jumping = false;
    private bool _stepDone = false;
    private bool _landing = false;
    private float _jumpingTime = 0f;
    public float AfterJumpCheckGroundTimeLimit = 0.2f;

    private Animator _animator;
    private void Awake()
    {
        //_leftlegTrm = _leftSolver.transform;
        //_rightlegTrm = _rightSolver.transform;
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();  
    }

    private void Start()
    {
        LandEvent.AddListener(() => _landing = false);    
    }

    public void Update()
    {
        if (_jumping) _jumpingTime += Time.deltaTime;

        float x = Input.GetAxisRaw("Horizontal");
        if (x != 0)
        {
            _isRight = x > 0;
            //FlipIk(_isRight);
            if (_isRight)
            {
                _inputState = EKeyInputState.Right;
                //StartCoroutine(WalkRight());
            }
            else
            {
                _inputState = EKeyInputState.Left;
                //StartCoroutine(WalkLeft());
            }
            _playerTrm.position += new Vector3(x, 0, 0) * m_Speed * Time.deltaTime;
        }
        else
        {
            _inputState = EKeyInputState.Idle;
            SetPrevDirIsRight(_isRight);
        }

        if(_jumping == true && _jumpingTime >= AfterJumpCheckGroundTimeLimit) //점프한지 0.2초가 흘ㄹ렀다면
        {
            if(GroundCheck())
            {
                PlayLandAnim(_inputState);
            }
        }

        if (_landing == true) return;
        if (_jumping == false && Input.GetKeyDown(KeyCode.Space)) 
            PlayJumpAnim(_inputState);
        if(_jumping == false)
            PlayMoveAnim(_inputState);
    }
    public void PlayLandAnim(EKeyInputState _input)
    {
        _landing = true;
        _jumping = false;
        switch (_input)
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
    public void PlayMoveAnim(EKeyInputState _input)
    {
        switch (_input)
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
    public void PlayJumpAnim(EKeyInputState _input)
    {
        _jumping = true;
        switch(_input)
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
        _jumpingTime = 0f;
        Debug.Log("Jump");
    }

    public void Jump()
    {
        _rb.AddForce(Vector2.up * JumpPower, ForceMode2D.Impulse);
    }
    
    public void PlayJumpEvent() => JumpEvent?.Invoke();
    public void PlayLandEvent() => LandEvent?.Invoke();
  
    private Vector2 NextStepPos()
    {
        Vector2 dir = _isRight ?
          Vector2.down + Vector2.right * StepWeight :
          Vector2.down + Vector2.left * StepWeight;
        RaycastHit2D hit = Physics2D.Raycast(_playerTrm.position, dir);
        return hit.collider != null ? hit.point : Vector2.zero;
    }

    public bool GroundCheck() =>
        Physics2D.OverlapCircle(_playerTrm.position, PositionRadius, _groundLayer);

    public void SetPrevDirIsRight(bool value)
    {
        if (_prevDirIsRight != value) _prevDirIsRight = value;
    }

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
