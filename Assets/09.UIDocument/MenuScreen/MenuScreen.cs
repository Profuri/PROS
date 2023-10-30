using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuScreen : MonoBehaviour
{
    private UIDocument _uiDocument;

    private VisualElement _popupContainer;
    private TextField _nameField;

    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        VisualElement root = _uiDocument.rootVisualElement;

        _popupContainer = root.Q<VisualElement>("popup-container");
        _popupContainer.RemoveFromClassList("off");

        _nameField = root.Q<TextField>("name-field");
        root.Q<Button>("btn-confirm").RegisterCallback<ClickEvent>(HandleNameConfirm);
    }

    private void HandleNameConfirm(ClickEvent evt)
    {
        _popupContainer.AddToClassList("off");
    }
}
