using UnityEngine;
using UnityEngine.Events;

public class UGUIMenu : MonoBehaviour
{
    private float _indexPosition;
    public float IndexPosition => _indexPosition;

    public UnityEvent CallBackEvent = null;

    public void SetUp()
    {
        Canvas.ForceUpdateCanvases();
        _indexPosition = ((RectTransform)transform).anchoredPosition.y;
    }

    public void OnAction()
    {
        CallBackEvent?.Invoke();
    }
}