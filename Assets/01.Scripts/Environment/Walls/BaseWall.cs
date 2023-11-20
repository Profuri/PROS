using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class BaseWall : MonoBehaviour
{
    private Vector3 _initPos;
    
    private Collider2D _collider;
    private Rigidbody2D _rigidbody;

    public Collider2D Collider => _collider;
    public Rigidbody2D Rigidbody => _rigidbody;

    public virtual void Init()
    {
        _initPos = transform.position;
        _collider = GetComponent<Collider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public virtual void Reset()
    {
        _collider.enabled = false;
        _rigidbody.velocity = Vector2.zero;
        transform.position = _initPos;
        _collider.enabled = true;
    }
}
