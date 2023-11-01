using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
[RequireComponent(typeof(Collider2D))]
public class PlayerBrain : MonoBehaviour
{
    private List<PlayerHandler> _handlers;
    
    private Collider2D _collider;
    public Collider2D Collider => _collider;

    [SerializeField] private InputSO _inputSO;
    public InputSO InputSO => _inputSO;
    [SerializeField] private MovementSO _movementSO;
    public MovementSO MovementSO => _movementSO;
    public PlayerMovement PlayerMovement => _playerMovement;
    private PlayerMovement _playerMovement;

    private Vector3 _mousePos;
    public Vector3 MousePos => _mousePos;
    private void Awake()
    {
        _handlers = new List<PlayerHandler>();
        GetComponents(_handlers);
        _collider = GetComponent<Collider2D>();
        
        _handlers.ForEach(h => h.Init(this));
        _playerMovement = GetHandlerComponent<PlayerMovement>();
        
        _inputSO.OnMouseAim += (mousePos) => _mousePos = mousePos;
    }

    public delegate void UnityMessageListener();

    public event UnityMessageListener OnEnableEvent;
    public event UnityMessageListener OnDisableEvent;
    public event UnityMessageListener OnUpdateEvent;
    public event UnityMessageListener OnFixedUpdateEvent;
    private void OnEnable() => OnEnableEvent?.Invoke();
    private void OnDisable() => OnDisableEvent?.Invoke();
    private void Update() => OnUpdateEvent?.Invoke();
    private void FixedUpdate() => OnFixedUpdateEvent?.Invoke();
    
    public T GetHandlerComponent<T>() where T : PlayerHandler
    {
        var test = _handlers.Find(handle => handle.GetType() == typeof(T)) as T;
        if(test == null) Debug.LogError("Can't Get HandlerComponent!");
        return test;
    }
    
    public string Attack(string a) => a;
    public delegate void CallFunc();
}
