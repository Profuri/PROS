using Unity.VisualScripting;
using UnityEngine;

public abstract class UGUIComponent : PoolableMono, IUGUI
{
    private Transform _prevParent;

    private Transform _parent;
    public Transform Parent => _parent;
    
    private EGenerateOption _options;
    public EGenerateOption Options => _options;

    private bool _isActive;
    public bool IsActive => _isActive;
    
    public virtual void GenerateUI(Transform parent, EGenerateOption options)
    {
        _prevParent = transform.parent;
        _options = options;

        if (options.HasFlag(EGenerateOption.CLEAR_PANEL))
        {
            UIManager.Instance.ClearPanel();
        }
        
        transform.SetParent(parent);

        if (options.HasFlag(EGenerateOption.RESETING_POS))
        {
            ((RectTransform)transform).offsetMin = Vector2.zero;
            ((RectTransform)transform).offsetMax = Vector2.zero;
        }

        _isActive = true;
    }

    public virtual void RemoveUI()
    {
        _isActive = false;
        transform.SetParent(_prevParent);
        PoolManager.Instance.Push(this);
    }

    public sealed override void Init()
    {
        // Do Nothing
    }
    
    public abstract void UpdateUI();
}