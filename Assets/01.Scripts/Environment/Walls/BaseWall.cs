using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class BaseWall : MonoBehaviour
{
    protected Collider2D _collider2D;
    protected Rigidbody2D _rigidbody2D;

    protected virtual void Awake()
    {
        _collider2D = GetComponent<Collider2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }
}
