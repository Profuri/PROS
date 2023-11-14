using UnityEngine;

public abstract class UGUIComponent : PoolableMono, IUGUI
{
    private Transform _prevParent;
    
    private Transform _parent;
    public Transform Parent => _parent;
    
    private EGenerateOption _options;
    public EGenerateOption Options => _options;
    
    public void GenerateUI(Transform parent, EGenerateOption options)
    {
        _prevParent = transform.parent;
        _options = options;

        if (options.HasFlag(EGenerateOption.CLEAR_PANEL))
        {
            UIManager.Instance.ClearPanel();
        }

        if (options.HasFlag(EGenerateOption.BLUR))
        {
            UIManager.Instance.SetBlur(true);
        }
        
        transform.SetParent(parent);
    }

    public virtual void RemoveUI()
    {
        if (_options.HasFlag(EGenerateOption.BLUR))
        {
            UIManager.Instance.SetBlur(false);
        }
        
        transform.SetParent(_prevParent);
        PoolManager.Instance.Push(this);
    }

    public abstract override void Init();
    public abstract void UpdateUI();
}