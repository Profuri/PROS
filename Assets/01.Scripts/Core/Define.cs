using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public static class Define
{
    private static Camera _mainCam;

    public static Camera MainCam
    {
        get
        {
            if (_mainCam == null)
            {
                _mainCam = GameObject.FindObjectOfType<Camera>();
            }

            return _mainCam;
        }
    }

    private static CinemachineVirtualCamera _vCam;
    public static CinemachineVirtualCamera VCam
    {
        get
        {
            if(_vCam == null)
            {
                _vCam = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
            }
            return _vCam;
        }
    }
}
