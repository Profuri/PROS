using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerController : MonoBehaviour
{
    public Animator Animator;
    public Rigidbody2D rb;
    public float jumpForce;
    public float playerSpeed;
    public Vector2 jumpHeight;
    private bool isOnGround;
    public float positionRaidus;
    public LayerMask ground;
    public Transform playerPos;

    private void Start()
    {
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();

        for(int i = 0; i < colliders.Length; i++)
        {
            for(int k = i + 1; k < colliders.Length; k++)
            {
                Physics2D.IgnoreCollision(colliders[i], colliders[k]);
            }
        }
    }

    private void Update()
    {
        if(Input.GetAxisRaw("Horizontal") != 0)
        {
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                Animator.Play("Walk");
                rb.AddForce(Vector2.right * playerSpeed);   
            }
            else
            {
                Animator.Play("WalkBack");
                rb.AddForce(Vector2.left * playerSpeed);
            }
        }
        else
        {
            Animator.Play("Idle");
        }

        isOnGround = Physics2D.OverlapCircle(playerPos.position, positionRaidus, ground);
        if(isOnGround == true && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector2.up * jumpForce);
        }
    }


}
