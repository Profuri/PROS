using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisiblePlatform : BasePlatform
{
    private bool _isVisible = true;
    public bool IsVisible => _isVisible;

    [SerializeField] private float _duration;

    private void Update()
    {
        // Debug
        if(Input.GetKeyDown(KeyCode.S))
        {
            SetVisible(true);
        }
        if(Input.GetKeyDown(KeyCode.H))
        {
            SetVisible(false);
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
