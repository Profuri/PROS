using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using static Define;
using PlayerManager = MonoPlayer.PlayerManager;
using Random = UnityEngine.Random;
using System;

public class PlayerBrain : MonoBehaviour
{
    private List<PlayerHandler> _handlers;

    [SerializeField] private Transform _agentTrm;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private InputSO _inputSO;
    [SerializeField] private MovementSO _movementSO;
    
    public Action OnOTC;
    
    #region Property
    public PhotonView PhotonView { get; private set; }
    public PlayerMovement PlayerMovement { get; private set; }
    public PlayerOTC PlayerOTC { get; private set; }
    public PlayerDefend PlayerDefend { get; private set; }
    public PlayerActionData ActionData { get; private set; }
    public PlayerColor PlayerColor { get; private set; }
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
        GetComponentsInChildren(_handlers);

        PhotonView = GetComponent<PhotonView>();
        ActionData = GetComponent<PlayerActionData>();
        PlayerOTC = GetHandlerComponent<PlayerOTC>();
        AnimationController = GetComponent<AnimationController>();
        PlayerDefend = GetHandlerComponent<PlayerDefend>();
        PlayerMovement = GetHandlerComponent<PlayerMovement>();
        PlayerColor = GetHandlerComponent<PlayerColor>();

        _handlers.ForEach(h => h.Init(this));

        OriginGravityScale = _rigidbody.gravityScale;

        _inputSO.OnMouseAim += AimToWorldPoint;
        OnDisableEvent += () => _inputSO.OnMouseAim -= AimToWorldPoint;
    }

    public void OnPlayerDead()
    {
        PlayerManager.Instance.RemovePlayer(PhotonView.Owner);
    }

    #region UnityMessage
    public delegate void UnityMessageListener();
    
    public event UnityMessageListener OnEnableEvent;
    public event UnityMessageListener OnDisableEvent;
    public event UnityMessageListener OnUpdateEvent;
    public event UnityMessageListener OnFixedUpdateEvent;

    private void OnEnable() { if (IsMine) { OnEnableEvent?.Invoke(); } }
    private void Update() { if (IsMine) { OnUpdateEvent?.Invoke(); } }
    private void OnDisable() { if(IsMine) { OnDisableEvent?.Invoke();} }
    private void FixedUpdate() { if (IsMine) { OnFixedUpdateEvent?.Invoke(); } }
    #endregion
    
    private T GetHandlerComponent<T>() where T : PlayerHandler
    {
        var test = _handlers.Find(handle => handle.GetType() == typeof(T)) as T;
        if (test == null) Debug.LogError("Can't Get HandlerComponent!");
        return (T)test;
    }

    private void AimToWorldPoint(Vector2 mousePos)
    {
        Vector3 worldMousePos = MainCam.ScreenToWorldPoint(mousePos);
        worldMousePos.z = 0f;
        MousePos = worldMousePos;
    }
    public void EnablePlayerHandlers(bool value) => _handlers.ForEach(h => h.enabled = value);
    //public void SetRagdollColsEnable(bool active) => _ragdollCols.ForEach(c => c.enabled = active);
    public void SetName(string nickName) => PhotonView.RPC("SetNameRPC", RpcTarget.All, nickName);
    [PunRPC]
    private void SetNameRPC(string nickName) => this.gameObject.name = nickName;
}