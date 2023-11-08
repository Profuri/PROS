using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using static Define;
public class PlayerBrain : MonoBehaviour
{
    private List<PlayerHandler> _handlers;
    [SerializeField] private Transform _agentTrm;
    public Transform AgentTrm => _agentTrm;
    
    [SerializeField] private Collider2D _collider;
    [SerializeField] private List<Collider2D> _ragdollCols;
    public Collider2D Collider => _collider;
    
    [SerializeField] private Rigidbody2D _rigidbody;
    public Rigidbody2D Rigidbody => _rigidbody;

    [SerializeField] private InputSO _inputSO;
    public InputSO InputSO => _inputSO;
    [SerializeField] private MovementSO _movementSO;
    public MovementSO MovementSO => _movementSO;

    private PhotonView _photonView;
    public PhotonView PhotonView => _photonView;
    public bool IsMine => _photonView.IsMine;
    private PlayerMovement _playerMovement;
    public PlayerMovement PlayerMovement => _playerMovement;

    private PlayerOTC _playerOTC;
    public PlayerOTC PlayerOTC => _playerOTC;

    private PlayerActionData _playerActionData;
    public PlayerActionData ActionData => _playerActionData;

    private Vector3 _mousePos;
    public Vector3 MousePos => _mousePos;
    private void Awake()
    {
        _handlers = new List<PlayerHandler>();
        GetComponentsInChildren(_handlers);
        
        //_collider = GetComponent<Collider2D>();
        //_rigidbody = GetComponent<Rigidbody2D>();
        
        _photonView = GetComponent<PhotonView>();
        _playerActionData = GetComponent<PlayerActionData>();
        _playerOTC = GetComponent<PlayerOTC>();
        
        _handlers.ForEach(h => h.Init(this));
        _playerMovement = GetHandlerComponent<PlayerMovement>();
        
        
        _inputSO.OnMouseAim += AimToWorldPoint;
        OnDisableEvent += () => _inputSO.OnMouseAim -= AimToWorldPoint;
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

    private void AimToWorldPoint(Vector2 mousePos)
    {
        Vector3 worldMousePos = MainCam.ScreenToWorldPoint(mousePos);
        worldMousePos.z = 0f;
        _mousePos = worldMousePos;
    }
    public void SetRagdollColsEnable(bool active) => _ragdollCols.ForEach(c => c.enabled = active);
    public void SetName(string nickName) => _photonView.RPC("SetNameRPC",RpcTarget.All,nickName);
    [PunRPC]
    private void SetNameRPC(string nickName) => this.gameObject.name = nickName;
}
