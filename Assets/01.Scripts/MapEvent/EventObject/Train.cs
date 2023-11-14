using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Train : BaseEventObject
{
    [SerializeField] private float _speed = 300f;
    [SerializeField] private float _prevTimeDelay = 5f;
    [SerializeField] private float _trainDashTime = 3f;
    [SerializeField] private LayerMask _layerMask;
    
    private Collider2D _collider;

    public override void Init()
    {
        base.Init();
        _collider = GetComponent<Collider2D>();
        int random = Random.Range(0,2);

        Vector3 direction = random > 0 ? Vector3.left : Vector3.right;
        _photonView.RPC(nameof(ThroughMapRPC),RpcTarget.All,direction);
    }
    protected override void DestroyObject()
    {
        PhotonNetwork.Destroy(this.gameObject);
    }

    [PunRPC]
    private void ThroughMapRPC(Vector3 dir)
    {
        StartCoroutine(ThroughMapCor(dir));
    }
    private IEnumerator ThroughMapCor(Vector3 dir)
    {
        //Todo: Notice train will be arrived
        yield return new WaitForSeconds(_prevTimeDelay);
        float timer = 0f;
        while (timer < _trainDashTime)
        {
            timer += Time.deltaTime;
            transform.position += dir * Time.deltaTime * _speed;
            
            Collider2D[] cols = Physics2D.OverlapBoxAll(transform.position,_collider.bounds.size,0f,_layerMask);
            if (cols.Length > 0)
            {
                foreach (Collider2D col in cols)
                {
                    if (col.TryGetComponent(out PlayerBrain playerBrain))
                    {
                        GameManager.Instance.OTCPlayer(playerBrain.PhotonView.Owner,Vector3.up);
                    }
                }
            }
                        
            yield return null;
        }
        DestroyObject();
    }
}
