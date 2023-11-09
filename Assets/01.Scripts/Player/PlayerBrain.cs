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

    //[SerializeField] private List<Collider2D> _ragdollCols;
    [SerializeField] private Transform _agentTrm;
    [SerializeField] private Collider2D _collider;
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
        GetComponentsInChildren(_handlers);

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

    public void Init(Vector3 spawnPos) => PhotonView.RPC("InitRPC", RpcTarget.All, spawnPos);

    [PunRPC]
    private void InitRPC(Vector3 spawnPos)
    {
        _collider.enabled = true;

        _rigidbody.gravityScale = OriginGravityScale;
        _rigidbody.velocity = Vector3.zero;

        PlayerMovement.IsStopped = false;
        transform.position = spawnPos;
    }
    #region UnityMessage
    public delegate void UnityMessageListener();
    public event UnityMessageListener OnEnableEvent;
    public event UnityMessageListener OnDisableEvent;
    public event UnityMessageListener OnUpdateEvent;
    public event UnityMessageListener OnFixedUpdateEvent;
    private void OnEnable() => OnEnableEvent?.Invoke();
    private void OnDisable() => OnDisableEvent?.Invoke();
    private void Update() => OnUpdateEvent?.Invoke();
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
    //public void SetRagdollColsEnable(bool active) => _ragdollCols.ForEach(c => c.enabled = active);
    public void SetName(string nickName) => PhotonView.RPC("SetNameRPC", RpcTarget.All, nickName);
    [PunRPC]
    private void SetNameRPC(string nickName) => this.gameObject.name = nickName;

    public void Revive()
    {
        PhotonView.RPC("ReviveRPC", RpcTarget.All);
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
        _rigidbody.gravityScale = OriginGravityScale;
    }
}
