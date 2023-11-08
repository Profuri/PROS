using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using static Define;
using System;
using Random = UnityEngine.Random;

public class PlayerBrain : MonoBehaviour
{
    private List<PlayerHandler> _handlers;

    private float _originGravityScale;
    public float OriginGravityScale => _originGravityScale;
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

        _photonView = GetComponent<PhotonView>();
        _playerActionData = GetComponent<PlayerActionData>();
        _playerOTC = GetComponent<PlayerOTC>();
        
        _handlers.ForEach(h => h.Init(this));
        _playerMovement = GetHandlerComponent<PlayerMovement>();

        _originGravityScale = _rigidbody.gravityScale;
        
        _inputSO.OnMouseAim += AimToWorldPoint;
        OnDisableEvent += () => _inputSO.OnMouseAim -= AimToWorldPoint;
    }
    
    public void Init(Vector3 spawnPos)
    {
        transform.position = spawnPos;
        _collider.enabled = true;
        _rigidbody.gravityScale = _originGravityScale;
        _rigidbody.velocity = Vector3.zero;
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

    public void Revive()
    {
        _photonView.RPC("ReviveRPC", RpcTarget.Others);
    }

    [PunRPC]
    private void ReviveRPC()
    {
        Transform points = GameObject.Find("Level/Points/SpawnPoints").transform;
        Vector3 pos = points.GetChild(Random.Range(0, points.childCount)).position;
        _rigidbody.gravityScale = 0;

        StartCoroutine(BlinkAndDrop(pos));
    }

    private IEnumerator BlinkAndDrop(Vector3 spawnPos)
    {
        var blink = new WaitForSeconds(0.2f);
        var term = new WaitForSeconds(0.4f);

        yield return new WaitForSeconds(1.5f);

        TrailRenderer tr = transform.Find("Trail").GetComponent<TrailRenderer>();
        tr.Clear();
        _rigidbody.velocity = Vector3.zero;
        transform.position = spawnPos;
        SpriteRenderer sp = transform.Find("Visual").GetComponent<SpriteRenderer>();
        for (int i = 0; i < 3; i++)
        {
            Color old = sp.color;
            old.a = 0.25f;
            sp.color = old;
            yield return blink;
            old.a = 1f;
            sp.color = old;
            yield return term;
        }

        Collider.enabled = true;
        _rigidbody.gravityScale = _originGravityScale;
    }
}
