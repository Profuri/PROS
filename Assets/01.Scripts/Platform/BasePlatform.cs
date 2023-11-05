using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePlatform : MonoBehaviour
{
    protected SpriteRenderer _spriteRenderer;
    protected Collider2D _collider2D;

    protected virtual void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider2D = GetComponent<Collider2D>();
    }
}
