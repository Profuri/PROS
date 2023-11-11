using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeController : MonoBehaviour
{
    private PlayerBrain _brain;
    public PlayerBrain Brain => _brain; 
    public bool OnHead { get; private set; } = false;

    private Camera _mainCam;
    public Camera MainCam => _mainCam;

    private void Awake()
    {
        _brain = GetComponent<PlayerBrain>();
        _mainCam = Camera.main;
    }

    private void OnMouseOver()
    {
        OnHead = true;    
    }

    private void OnMouseExit()
    {
        OnHead = false;
    }
}
