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
    [SerializeField] float _stepwait = 0.3f; 
    [SerializeField] LayerMask _ground;
    [SerializeField] Transform _playerPos;
    [SerializeField] float positionRadius;

    private bool _isOnGround;

    private void Update()
    {
        _isOnGround = Physics2D.OverlapCircle(_playerPos.position, positionRadius, _ground);
        if (_isOnGround == true && Input.GetKeyDown(KeyCode.Space))
        {
            _animator.Play("Jump");
            Debug.Log("Jump");
            // _rb.AddForce(Vector2.up * _jumpForce);
        }
        if (_isOnGround == true) return;

        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                _animator.Play("WalkRight");

                _rightRb.AddForce(Vector2.right * _playerSpeed * Time.deltaTime, ForceMode2D.Impulse);
                _leftRb.AddForce(Vector2.right * _playerSpeed * Time.deltaTime, ForceMode2D.Impulse);
                StartCoroutine(MoveRight(_stepwait));
               
                //transform.position += new Vector3(moveValue.x, moveValue.y);
                //rb.AddForce(moveValue);
            }
            else
            {
                _rightRb.AddForce(Vector2.left * _playerSpeed * Time.deltaTime, ForceMode2D.Impulse);
                _leftRb.AddForce(Vector2.left * _playerSpeed * Time.deltaTime, ForceMode2D.Impulse);
                _animator.Play("WalkLeft");
                StartCoroutine(MoveLeft(_stepwait));
                //transform.position += new Vector3(moveValue.x, moveValue.y);
                //rb.AddForce(moveValue);
            }
        }
        else
        {
            _leftRb.velocity = new Vector3(0, _leftRb.velocity.y);
            _rightRb.velocity = new Vector3(0, _rightRb.velocity.y);
            //_leftRb.velocity = Vector3.zero;
            //_rightRb.velocity = Vector3.zero;
            _animator.Play("Idle");
        }


    }

    public void Jump()
    {
        _rb.AddForce(Vector2.up * _jumpForce);
    }

    IEnumerator MoveRight(float waitTime)
    {
        _rightRb.AddRelativeForce(Vector2.right * _playerSpeed * Time.deltaTime);
        //_rightRb.AddForce(Vector2.right * _playerSpeed * Time.deltaTime, ForceMode2D.Impulse);
        yield return new WaitForSeconds(waitTime);
        _leftRb.AddRelativeForce(Vector2.right * _playerSpeed * Time.deltaTime);
    }

    IEnumerator MoveLeft(float waitTime)
    {
        _leftRb.AddRelativeForce(Vector2.left * _playerSpeed * Time.deltaTime);
        yield return new WaitForSeconds(waitTime);
        _rightRb.AddRelativeForce(Vector2.left * _playerSpeed * Time.deltaTime);
    }

}
