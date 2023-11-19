using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance {
        get
        {
            if (_instance is null)
            {
                _instance = FindObjectOfType<UIManager>();
            }
            return _instance;
        }
    }

    [SerializeField] private Canvas _mainCanvas;
    public Canvas MainCanvas => _mainCanvas;
    
    private Stack<UGUIComponent> _componentStack;
    public UGUIComponent TopComponent => _componentStack.Peek();

    public void Init()
    {
        _componentStack = new Stack<UGUIComponent>();
        GenerateUGUI("MenuSceneScreen", EGenerateOption.STACKING | EGenerateOption.RESETING_POS);
        GenerateUGUI("BlockScreen", EGenerateOption.STACKING | EGenerateOption.RESETING_POS);
        var loading = GenerateUGUI("LoadingScreen", EGenerateOption.STACKING | EGenerateOption.RESETING_POS) as LoadingScreen;
        loading.ExecuteLoading(ELoadingType.SERVER_CONNECT, RemoveTopUGUI);
    }

    private void Update()
    {
        if (_componentStack.Count > 0)
        {
            TopComponent.UpdateUI();
        }
    }

    public UGUIComponent GenerateUGUI(string componentName, EGenerateOption options = EGenerateOption.NONE, Transform parent = null)
    {
        if (parent == null)
        {
            parent = _mainCanvas.transform;
        }
        
        var ugui = PoolManager.Instance.Pop(componentName) as UGUIComponent;

        if (ugui is null)
        {
            return null;
        }

        ugui.GenerateUI(parent, options);

        if (options.HasFlag(EGenerateOption.STACKING))
        {
            _componentStack.Push(ugui);
        }
        
        return ugui;
    }

    public void RemoveTopUGUI()
    {
        var top = _componentStack.Pop();
        top.RemoveUI();
    }

    public void ReturnUGUI()
    {
        var curComponent = _componentStack.Pop();
        var prevComponent = _componentStack.Pop();

        if (curComponent == null)
        {
            Debug.LogWarning("There is not exist current UGUI");
            return;
        }
        
        if (prevComponent == null)
        {
            Debug.LogWarning("There is not exist prev UGUI");
            return;
        }
        
        curComponent.RemoveUI();
        GenerateUGUI(prevComponent.name, prevComponent.Options, prevComponent.Parent);
    }

    public void ClearPanel()
    {
        while (_componentStack.Count > 0)
        {
            _componentStack.Pop().RemoveUI();
        }
    }

    public void SetBlur(bool value)
    {
        Debug.Log(value ? "blur on!" : "blur off!");
    }
}