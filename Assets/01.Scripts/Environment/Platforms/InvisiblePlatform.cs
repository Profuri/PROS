using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisiblePlatform : BasePlatform
{
    [SerializeField] private bool _isVisible = true;
    public bool IsVisible => _isVisible;

    [SerializeField] private float _duration = 0.3f;
    [SerializeField] private float _term = 3f;
    [SerializeField] private float _delay = 0f; 

    protected override void Awake()
    {
        base.Awake();
        SetVisible(_isVisible);
    }

    private void Start()
    {
        StartCoroutine(ChangeVisibleCoroutine());
    }

    private IEnumerator ChangeVisibleCoroutine()
    {
        yield return new WaitForSeconds(_delay);
        while(true)
        {
            yield return new WaitForSeconds(_term);
            SetVisible(!_isVisible);
        }
    }

    public void SetVisible(bool value)
    {
        if (value == _isVisible) return;

        _isVisible = value;
        ShowSpriteRenderer(value);
        _collider2D.enabled = value;
    }

    private void ShowSpriteRenderer(bool value)
    {
        if(value)
        { 
            _spriteRenderer.DOFade(1f, _duration);
            _spriteRenderer.enabled = true;
        }
        else
        {
            _spriteRenderer.DOFade(0f, _duration)
                .OnComplete(() => _spriteRenderer.enabled = false);
        }
    }
}
