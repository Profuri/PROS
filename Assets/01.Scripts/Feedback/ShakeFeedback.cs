using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShakeFeedback : Feedback
{

    [SerializeField]
    private float _duration = 0.2f, _strength = 1f, _randomness = 90f;
    [SerializeField]
    private int _vibrato = 10;

    [SerializeField]
    private bool _snapping = false,_fadeOut = false;

    public override void CreateFeedback()
    {
        Define.VCam.transform.DOShakePosition(_duration, _strength, _vibrato,_randomness,_snapping,_fadeOut);
    }

    public override void FinishFeedback()
    {
        Define.VCam.transform.DOComplete();
    }
}
