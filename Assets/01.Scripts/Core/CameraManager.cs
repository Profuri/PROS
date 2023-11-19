using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager _instance;
    public static CameraManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<CameraManager>();
            }
            return _instance;
        }
    }

    private CinemachineVirtualCamera _vCam;
    public  CinemachineVirtualCamera VCam
    {
        get
        {
            if(_vCam)
            {
                _vCam = FindObjectOfType<CinemachineVirtualCamera>();
            }
            return _vCam;
        }
    }


    public void Init()
    {

    }

    public void ShakeCamera()
    {
        
    }
}
