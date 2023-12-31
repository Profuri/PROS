using System;
using System.Collections.Generic;
using UnityEngine;

public class UGUIMenuSystem : MonoBehaviour
{
    [SerializeField] private InputSO _inputSO;

    [SerializeField] private AudioClip _indexChangeClip;
    [SerializeField] private AudioClip _enterClip;
    
    private RectTransform _cursorRectTransform;

    private List<UGUIMenu> _menus;

    private UGUIComponent _ownerComponent;

    private int _index;
    public int Index => _index;

    public void SetUp(UGUIComponent ownerComponent)
    {
        _menus = new List<UGUIMenu>();
        GetComponentsInChildren(_menus);
        _menus.ForEach(menu => menu.SetUp());

        _ownerComponent = ownerComponent;

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
        if (UIManager.Instance.TopComponent != _ownerComponent)
        {
            return;
        }

        AudioManager.Instance.Play(_indexChangeClip);
        SetIndex(_index - 1);
    }

    private void IndexDown()
    {
        if (UIManager.Instance.TopComponent != _ownerComponent)
        {
            return;
        }
        
        AudioManager.Instance.Play(_indexChangeClip);
        SetIndex(_index + 1);
    }

    private void SetIndex(int index)
    {
        _index = index;
        
        if (_index < 0)
        {
            _index = _menus.Count - 1;
        }

        if (_index >= _menus.Count)
        {
            _index = 0;
        }
        
        _cursorRectTransform.anchoredPosition = new Vector2(0, _menus[_index].IndexPosition);
    }

    private void ActionCallBack()
    {
        if (UIManager.Instance.TopComponent != _ownerComponent)
        {
            return;
        }
        
        _menus[_index].OnAction();
        AudioManager.Instance.Play(_enterClip);
    }
}