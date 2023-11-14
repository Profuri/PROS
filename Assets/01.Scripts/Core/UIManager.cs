using System;
using System.Collections.Generic;
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
    public Stack<UGUIComponent> ComponentStack => _componentStack;

    public void Init()
    {
        _componentStack = new Stack<UGUIComponent>();
        GenerateUGUI("MenuSceneScreen");
    }

    private void Update()
    {
        var uguiComponent = _componentStack.Peek();
        if (uguiComponent is not null)
        {
            uguiComponent.UpdateUI();
        }
    }

    public void GenerateUGUI(string componentName, Transform parent = null, EGenerateOption options = EGenerateOption.NONE)
    {
        if (parent == null)
        {
            parent = _mainCanvas.transform;
        }
        
        var ugui = PoolManager.Instance.Pop(componentName) as UGUIComponent;

        if (ugui == null)
        {
            return;
        }
        
        ugui.GenerateUI(parent, options);
        _componentStack.Push(ugui);
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
        GenerateUGUI(prevComponent.name, prevComponent.Parent, prevComponent.Options);
    }

    public void ClearPanel()
    {
        var generatedComponents = new List<UGUIComponent>();
        _mainCanvas.GetComponentsInChildren(generatedComponents);

        foreach (var component in generatedComponents)
        {
            component.RemoveUI();
        }
    }

    public void SetBlur(bool value)
    {
        Debug.Log(value ? "blur on!" : "blur off!");
    }
}