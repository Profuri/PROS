using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    public static Camera _mainCam;

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

}
