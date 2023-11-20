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
    
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private ShadowCaster2D _shadowCaster;
    private PhotonView _photonView;

    public SpriteRenderer SpriteRenderer => _spriteRenderer;
    public Collider2D Collider => _collider;
    public ShadowCaster2D ShadowCaster => _shadowCaster;

    public virtual void Init(int index)
    {
        _index = index;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _shadowCaster = GetComponent<ShadowCaster2D>();
        _photonView = GetComponent<PhotonView>();
    }

    public virtual void Reset()
    {
        _collider.enabled = false;
        transform.position = _initPos;
        _collider.enabled = true;
    }
}
