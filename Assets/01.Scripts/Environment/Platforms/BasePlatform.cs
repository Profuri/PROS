using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class BasePlatform : MonoBehaviour
{
    private int _index;
    public int Index => _index;
    
    private Vector3 _initPos;
    private Quaternion _initRot;
    
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private ShadowCaster2D _shadowCaster;

    public SpriteRenderer SpriteRenderer => _spriteRenderer;
    public Collider2D Collider => _collider;
    public ShadowCaster2D ShadowCaster => _shadowCaster;

    public virtual void Init(int index)
    {
        _index = index;
        _initPos = transform.position;
        _initRot = transform.rotation;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _shadowCaster = GetComponent<ShadowCaster2D>();
    }

    public virtual void Reset()
    {
        transform.rotation = _initRot;
        transform.position = _initPos;
    }
}
