using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EyeTracking : MonoBehaviour
{
    public Transform _player;
    public float rotationSpeed = 5f; // ȸ�� �ӵ� ����
    public float eyeSpeed = 5f; // ���� �̵� �ӵ�
    public float pupilSpeed = 10f; // �������� �̵� �ӵ�
    public float _moveFieldScale = 1;
    private PlayerBrain _brain;
    private Transform _eyePupil; // �������� Transform

    Camera _mainCam;
    private void Awake()
    {
        _brain = _player.GetComponent<PlayerBrain>();
        _eyePupil = transform.GetChild(0).transform;
        _mainCam = Camera.main;
        Debug.Log(_mainCam);
    }

    void Update()
    {
        Vector2 mousePosition = _brain.InputSO.CurrentMousePos;
        Vector2 lookDir =  mousePosition - (Vector2)_mainCam.WorldToScreenPoint((Vector2)_eyePupil.position);
        lookDir.Normalize();
        
        float pupilAngle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
            
        _eyePupil.localPosition = Vector2.Lerp(_eyePupil.localPosition, _moveFieldScale * lookDir, Time.deltaTime * pupilSpeed);
        //_eyePupil.rotation = Quaternion.AngleAxis(pupilAngle, Vector3.forward);
       _eyePupil.localRotation = Quaternion.Lerp(_eyePupil.localRotation, Quaternion.Euler(0f, 0f, pupilAngle), Time.deltaTime * pupilSpeed);
    }
}
