using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuScreen : MonoBehaviour
{
    private UIDocument _uiDocument;

    [SerializeField] private VisualTreeAsset _playerTempalte;
    [SerializeField] private VisualTreeAsset _roomTemplate;



    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        VisualElement root = _uiDocument.rootVisualElement;

    }

    private void OnDisable()
    {

    }
}
