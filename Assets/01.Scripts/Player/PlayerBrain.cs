using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using static Define;
using System;
using Photon.Pun.Demo.PunBasics;
using PlayerManager = MonoPlayer.PlayerManager;
using Random = UnityEngine.Random;

public class PlayerBrain : MonoBehaviour
{
    private List<PlayerHandler> _handlers;

    [SerializeField] private Transform _agentTrm;
    [SerializeField] private Collider2D _collider;
    private List<Collider2D> _cols;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private InputSO _inputSO;
    [SerializeField] private MovementSO _movementSO;

    #region Property
    public PhotonView PhotonView { get; private set; }
    public PlayerMovement PlayerMovement { get; private set; }
    public PlayerOTC PlayerOTC { get; private set; }
    public PlayerDefend PlayerDefend { get; private set; }
    public PlayerActionData ActionData { get; private set; }
    public AnimationController AnimationController { get; private set; }
    public float OriginGravityScale { get; private set; }
    public Vector3 MousePos { get; private set; }

    public MovementSO MovementSO => _movementSO;
    public InputSO InputSO => _inputSO;
    public Rigidbody2D Rigidbody => _rigidbody;
    public Collider2D Collider => _collider;
    public Transform AgentTrm => _agentTrm;

    public bool IsMine => PhotonView.IsMine;
    #endregion

    private void Awake()
    {
        _handlers = new List<PlayerHandler>();
        _cols = new List<Collider2D>();
        GetComponentsInChildren(_handlers);
        GetComponentsInChildren(_cols);

        PhotonView = GetComponent<PhotonView>();
        ActionData = GetComponent<PlayerActionData>();
        PlayerOTC = GetComponent<PlayerOTC>();
        AnimationController = GetComponent<AnimationController>();
        PlayerDefend = GetComponent<PlayerDefend>();

        _handlers.ForEach(h => h.Init(this));
        PlayerMovement = GetHandlerComponent<PlayerMovement>();

        OriginGravityScale = _rigidbody.gravityScale;

        _inputSO.OnMouseAim += AimToWorldPoint;
        OnDisableEvent += () => _inputSO.OnMouseAim -= AimToWorldPoint;
    }

    public void OnPlayerDead()
    {
        Debug.Log("주금");
        PlayerManager.Instance.RemovePlayer(PhotonView.Owner);
    }

    #region UnityMessage
    public delegate void UnityMessageListener();
    public event UnityMessageListener OnEnableEvent;
    public event UnityMessageListener OnDisableEvent;
    public event UnityMessageListener OnUpdateEvent;
    public event UnityMessageListener OnFixedUpdateEvent;
    
    private void OnEnable() => OnEnableEvent?.Invoke();
    private void Update() => OnUpdateEvent?.Invoke();
    private void OnDisable() => OnDisableEvent?.Invoke(); 
    private void FixedUpdate() => OnFixedUpdateEvent?.Invoke();
    #endregion
    
    public T GetHandlerComponent<T>() where T : PlayerHandler
    {
        var test = _handlers.Find(handle => handle.GetType() == typeof(T)) as T;
        if (test == null) Debug.LogError("Can't Get HandlerComponent!");
        return test;
    }

    private void AimToWorldPoint(Vector2 mousePos)
    {
        Vector3 worldMousePos = MainCam.ScreenToWorldPoint(mousePos);
        worldMousePos.z = 0f;
        MousePos = worldMousePos;
    }
    
    public void SetColsTrigger(bool trigger) => _cols.ForEach(c => c.isTrigger = trigger);
    public void SetName(string nickName) => PhotonView.RPC("SetNameRPC", RpcTarget.All, nickName);
    [PunRPC]
    private void SetNameRPC(string nickName) => this.gameObject.name = nickName;
}