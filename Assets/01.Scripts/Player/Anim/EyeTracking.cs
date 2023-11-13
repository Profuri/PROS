using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EyeTracking : MonoBehaviour
{
    [SerializeField]
    private EyeController _eyeController;
    public float rotationSpeed = 5f; // 회전 속도 조절
    public float eyeSpeed = 5f; // 눈의 이동 속도
    public float pupilSpeed = 10f; // 눈동자의 이동 속도
    public float _moveFieldScale = 1;

    private Transform _eyePupil; // 눈동자의 Transform
    private Camera _mainCam;

    private void Awake()
    {
        _eyePupil = transform.GetChild(0).transform;
        _mainCam = Camera.main;
    }


    void Update()
    {
        //if (_eyeController.XScale < 0f) transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y);
        Vector2 mousePosition = _eyeController.Brain.InputSO.CurrentMousePos;
        Vector2 lookDir =  mousePosition - (Vector2)_eyeController.MainCam.WorldToScreenPoint((Vector2)_eyePupil.position);
        lookDir.Normalize();
        if (_eyeController.OnHead) lookDir = Vector2.zero;
        
        float pupilAngle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
            
        _eyePupil.localPosition = Vector2.Lerp(_eyePupil.localPosition, _moveFieldScale * lookDir, Time.deltaTime * pupilSpeed);
        //_eyePupil.rotation = Quaternion.AngleAxis(pupilAngle, Vector3.forward);
        _eyePupil.localRotation = Quaternion.Lerp(_eyePupil.localRotation, Quaternion.Euler(0f, 0f, pupilAngle), Time.deltaTime * pupilSpeed);
    }
}
