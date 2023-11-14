using System;
using System.Collections.Generic;
using UnityEngine;

public class UGUIMenuSystem : MonoBehaviour
{
    [SerializeField] private InputSO _inputSO;
    
    private RectTransform _cursorRectTransform;

    private List<UGUIMenu> _menus;

    private int _index;
    public int Index => _index;

    public void SetUp()
    {
        _menus = new List<UGUIMenu>();
        GetComponentsInChildren(_menus);
        _menus.ForEach(menu => menu.SetUp());

        _cursorRectTransform = (RectTransform)transform.Find("Cursor");

        _inputSO.OnMenuIndexUpPress += IndexUp;
        _inputSO.OnMenuIndexDownPress += IndexDown;
        _inputSO.OnEnterPress += ActionCallBack;
        
        SetIndex(0);
    }

    public void Release()
    {
        _inputSO.OnMenuIndexUpPress -= IndexUp;
        _inputSO.OnMenuIndexDownPress -= IndexDown;
        _inputSO.OnEnterPress -= ActionCallBack;
    }

    private void IndexUp()
    {
        SetIndex(_index - 1);
    }

    private void IndexDown()
    {
        SetIndex(_index + 1);
    }

    private void SetIndex(int index)
    {
        _index = Mathf.Clamp(index, 0, _menus.Count - 1);
        _cursorRectTransform.anchoredPosition = new Vector2(0, _menus[_index].IndexPosition);
    }

    private void ActionCallBack()
    {
        _menus[_index].OnAction();
    }
}