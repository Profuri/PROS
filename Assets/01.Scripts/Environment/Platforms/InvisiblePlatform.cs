using DG.Tweening;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisiblePlatform : BasePlatform
{
    [SerializeField] private bool _isVisible = true;

    public bool IsVisible
    {
        get => _isVisible;
        set => _isVisible = value;
    }

    [SerializeField] private float _duration = 0.3f;
    [SerializeField] private float _delay = 0f;

    private float _currentTime;

    public override void Init(int index)
    {
        base.Init(index);

        _currentTime = 0f;
        if (NetworkManager.Instance.IsMasterClient)
        {
            StartCoroutine(FadeRoutine());
        }
    }

    public override void Reset()
    {
        base.Reset();
        StopAllCoroutines();
    }

    private IEnumerator FadeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_delay);
            
            _currentTime += _isVisible ? -Time.deltaTime : Time.deltaTime;
            var percent = _currentTime / _duration;
            StageManager.Instance.SetOpacity(Index, percent);
            yield return null;
        }
    }
}
