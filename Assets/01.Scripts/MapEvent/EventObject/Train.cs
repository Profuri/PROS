using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using Random = UnityEngine.Random;

public class Train : BaseEventObject
{
    [SerializeField] private float _speed = 3000f;
    [SerializeField] private float _prevTimeDelay = 3f;
    [SerializeField] private float _trainDashTime = 3f;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public override void Init()
    {
        base.Init();
        int random = Random.Range(0,2);

        Vector3 direction = random > 0 ? Vector3.left : Vector3.right;
        _photonView.RPC(nameof(ThroughMapRPC),RpcTarget.All,direction);
    }
    
    protected override void DestroyObject()
    {
        if(NetworkManager.Instance.IsMasterClient)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    [PunRPC]
    private void ThroughMapRPC(Vector3 dir)
    {
        StartCoroutine(ThroughMapCor(dir));
    }
    private IEnumerator ThroughMapCor(Vector3 dir)
    {
        //Todo: Notice train will be arrived
        float prevTimer = 0f;

        Color alphaColor = _spriteRenderer.material.color;
        _collider.enabled = false;
        while(prevTimer <= _prevTimeDelay)
        {
            prevTimer += Time.deltaTime;
            alphaColor.a = (float)(Math.Sin(prevTimer * 10f) + 1) * 0.5f;
            _spriteRenderer.material.color = alphaColor;
            yield return null;
        }
        float timer = 0f;

        alphaColor.a = 1f;
        _spriteRenderer.material.color  = alphaColor;
        transform.position = transform.position - dir * _collider.bounds.size.x;
        _collider.enabled = true;
        while (timer <= _trainDashTime)
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
                        Vector3 attackDir = (col.transform.position - transform.position).normalized + dir;
                        GameManager.Instance.OTCPlayer(playerBrain.PhotonView.Owner,attackDir);
                    }
                }
            }
                        
            yield return null;
        }
        DestroyObject();
    }
}
