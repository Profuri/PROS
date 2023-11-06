using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerController : MonoBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] Rigidbody2D _leftRb;
    [SerializeField] Rigidbody2D _rightRb;
    [SerializeField] float _jumpForce;
    [SerializeField] float _playerSpeed;
    [SerializeField] LayerMask _ground;
    [SerializeField] Transform _playerPos;
    [SerializeField] float positionRadius;

    //[SerializeField] float _stepwait = 0.3f; 
    private bool _isOnGround = false;   
    private bool _isJumping = false;

    private void Update()
    {
        if (_isJumping) return;
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                float x = Input.GetAxisRaw("Horizontal");
                transform.position += new Vector3(x, 0) * _playerSpeed * Time.deltaTime;
                _animator.Play("WalkRight");
            }
            else
            {
                float x = Input.GetAxisRaw("Horizontal");
                transform.position += new Vector3(x, 0) * _playerSpeed * Time.deltaTime;
                _animator.Play("WalkLeft");
            }
        }
        else
        {
            _animator.Play("Idle");
        }

        _isOnGround = Physics2D.OverlapCircle(_playerPos.position, positionRadius, _ground);
        if (_isOnGround == true && Input.GetKeyDown(KeyCode.Space))
        {
            _isJumping = true;
            _animator.Play("Jump");
            _isOnGround = false;
        }


    }
    public void Jump()
    {
        _isJumping = false;
        _rb.AddForce(Vector2.up * _jumpForce);
        Debug.Log("Jump");
    }

    //IEnumerator MoveRight(float waitTime)
    //{
    //    //_rightRb.velocity = (Vector2.right * _playerSpeed * Time.deltaTime);
    //    _rightRb.AddRelativeForce(Vector2.right * _playerSpeed * Time.deltaTime, ForceMode2D.Force);
    //    //_rightRb.AddForce(Vector2.right * _playerSpeed * Time.deltaTime, ForceMode2D.Impulse);
    //    yield return new WaitForSeconds(waitTime);
    //    //_leftRb.velocity = (Vector2.right * _playerSpeed * Time.deltaTime);
    //    _leftRb.AddRelativeForce(Vector2.right * _playerSpeed * Time.deltaTime, ForceMode2D.Force);
    //}

    //IEnumerator MoveLeft(float waitTime)
    //{
    //    //_leftRb.velocity = (Vector2.left * _playerSpeed * Time.deltaTime);
    //    _leftRb.AddRelativeForce(Vector2.left * _playerSpeed * Time.deltaTime, ForceMode2D.Force);
    //    yield return new WaitForSeconds(waitTime);
    //    //_rightRb.velocity = (Vector2.left * _playerSpeed * Time.deltaTime);
    //    _rightRb.AddRelativeForce(Vector2.left * _playerSpeed * Time.deltaTime, ForceMode2D.Force);
    //}

}
