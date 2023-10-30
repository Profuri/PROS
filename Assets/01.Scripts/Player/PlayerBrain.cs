using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBrain : MonoBehaviour
{
    private List<PlayerHandler> _handlers;
    [SerializeField] private InputSO _inputSO;
    public InputSO InputSO => _inputSO;

    [SerializeField] private MovementSO _movemenetSO;
    public MovementSO MovementSO
    {
        get => _movemenetSO;
        set => _movemenetSO = value;
    }

    private void Awake()
    {
        _handlers = new List<PlayerHandler>();
        GetComponents(_handlers);
        
        _handlers.ForEach(h => h.Init(this));
    }

    public delegate void UnityMessageListener();

    public event UnityMessageListener OnEnableEvent;
    public event UnityMessageListener OnDisableEvent;
    public event UnityMessageListener OnUpdateEvent;
    public event UnityMessageListener OnFixedUpdateEvent;
    private void OnEnable() => OnEnableEvent?.Invoke();
    private void OnDisable() => OnUpdateEvent?.Invoke();
    private void Update() => OnUpdateEvent?.Invoke();
    private void FixedUpdate() => OnFixedUpdateEvent?.Invoke();

}
