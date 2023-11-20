using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeController : PlayerHandler
{
    public bool OnHead { get; private set; } = false;
    private List<EyeTracking> _eyeTrackingList;

    public override void Init(PlayerBrain brain)
    {
        base.Init(brain);

        _mainCam = Camera.main;

        _eyeTrackingList = new List<EyeTracking>();
        GetComponentsInChildren(_eyeTrackingList);
        _eyeTrackingList.ForEach(e => e.Init(this,brain));
    }

    private Camera _mainCam;
    public Camera MainCam => _mainCam;
    
    private void OnMouseOver()
    {
        OnHead = true;    
    }

    private void OnMouseExit()
    {
        OnHead = false;
    }

    public override void BrainUpdate()
    {
        _eyeTrackingList.ForEach(e => e.EyeUpdate());
    }

    public override void BrainFixedUpdate()
    {

    }

}
