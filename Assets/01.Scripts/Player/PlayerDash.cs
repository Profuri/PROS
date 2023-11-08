using System.Collections;
using MonoPlayer;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using static Ease;
public class PlayerDash : PlayerHandler
{
    private Coroutine _dashCoroutine;
    [SerializeField] private float _dashTime = 0.3f;
    public bool CanDash => !_brain.PlayerMovement.IsGrounded && !_isDashed;
    private bool _isDashed = false;

    [SerializeField] private LayerMask _damageableLayer;
    [SerializeField] private LayerMask _obstacleLayer;

    public override void Init(PlayerBrain brain)
    {
        base.Init(brain);
        _brain.InputSO.OnDashKeyPress += DashRPC;
        _brain.OnDisableEvent += () => _brain.InputSO.OnDashKeyPress -= DashRPC;
        StopAllCoroutines();
    }

    #region Dash
    private void DashRPC()
    {
        if (_brain.IsMine)
        {
            Vector3 mouseDir = (_brain.MousePos - transform.position).normalized;
            _brain.PhotonView.RPC("Dash", RpcTarget.All, mouseDir);
        }
    }
    [PunRPC]
    private void Dash(Vector3 mouseDir)
    {
        if (CanDash)
        {
            if (_dashCoroutine != null)
            {
                StopCoroutine(_dashCoroutine);
            }
            float dashPower = _brain.MovementSO.DashPower;
            _isDashed = true;

            _dashCoroutine = StartCoroutine(DashCoroutine(dashPower, mouseDir));
        }
    }
    private IEnumerator DashCoroutine(float power, Vector3 mouseDir)
    {
        float prevValue = 0f;
        float timer = 0f;

        Vector3 destination = transform.position + mouseDir * power;
        float distanceFromDestination = Vector3.Distance(transform.position, destination);

        float timeToArrive = distanceFromDestination / power * _dashTime;

        float radius = _brain.Collider.bounds.size.x * 0.5f;

        _brain.ActionData.IsDashing = true;


        float percent = 0f;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, mouseDir, distanceFromDestination, _obstacleLayer);

        if (hit.collider != null)
        {
            var obstacleCollider = hit.collider;
            destination = hit.point - (Vector2)mouseDir * (_brain.Collider.bounds.size / 2);
            timeToArrive = Vector3.Distance(transform.position, destination) / power * _dashTime;
        }

        //��ǥ ��ġ���� ���� �ð��� �뽬 Ÿ�Ӹ�ŭ ����� 0 ~ 1�� �������.
        //�� ��ġ���� �浹üŬ�� ���ְ� �����̼��� ������.
        //����ε� PLAYER�� Brain�� Player�� ã�ƿ�.
        _brain.PlayerMovement.StopImmediately(timeToArrive);
        while (timer < timeToArrive)
        {
            timer += Time.deltaTime;

            percent = timer / timeToArrive;

            float easingValue = EaseOutCubic(percent);
            float stepEasingValue = easingValue - prevValue;

            prevValue = easingValue;

            transform.position = Vector3.Lerp(transform.position, destination, stepEasingValue);

            _brain.PlayerMovement.SetRotationByDirection(mouseDir, easingValue);


            //CheckCollisionRealtime
            //���� �÷��̾ �����̸鼭 �ε����� ���� Ȯ���ϴ� �ڵ�.
            Collider2D collider = Physics2D.OverlapCircle(transform.position, radius, _damageableLayer);


            //ã�� �ݶ��̴��� �� �ݶ��̴��� �ƴϿ��� ��.
            if (collider != default(Collider2D) && collider.Equals(_brain.Collider) == false)
            {
                if (collider.TryGetComponent(out PlayerBrain brain))
                {
                    var player = brain.PhotonView.Owner;
                    //_brain.PlayerMovement.StopAllCoroutines();
                    _brain.PlayerMovement.StopImmediately(0f);

                    _brain.PhotonView.RPC("OTCPlayer", RpcTarget.All, player, mouseDir);
                    if (brain.PlayerDefend.IsDefend)
                    {
                        _brain.PhotonView.RPC("OTCPlayer", RpcTarget.All, photonView.Owner, -mouseDir);
                    }
                    
                    yield break;
                }
            }
            yield return null;
        }

        //���� ������ �浹üũ�� �� �� �� ����.
        _brain.Rigidbody.velocity = Vector3.zero;
        _brain.ActionData.IsDashing = false;
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius * 1.3f, _damageableLayer);
        if (cols.Length > 0)
        {
            foreach (var col in cols)
            {
                if (col.Equals(_brain.Collider) == false)
                {
                    if (col.TryGetComponent(out PlayerBrain brain))
                    {
                        var player = brain.PhotonView.Owner;

                        _brain.PhotonView.RPC("OTCPlayer", RpcTarget.All, player, mouseDir);
                        if (brain.PlayerDefend.IsDefend)
                        {
                            _brain.PhotonView.RPC("OTCPlayer", RpcTarget.All, photonView.Owner, -mouseDir);
                        }
                    }
                }
            }
        }
    }
    #endregion


    [PunRPC]
    private void OTCPlayer(Player player, Vector3 attackDir)
    {
        if (player == photonView.Owner) _brain.PlayerDefend.IsDefendBounce = true;
        PlayerManager.Instance.OTCPlayer(player, attackDir);
        transform.rotation = Quaternion.identity;
    }
    public override void BrainUpdate()
    {
        if (_brain.PlayerMovement.IsGrounded) _isDashed = false;
    }

    public override void BrainFixedUpdate()
    {
    }
}
