using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ObstaclePlatform : BasePlatform
{
    private int _targetLayer;

    public override void Init(int index)
    {
        base.Init(index);
        _targetLayer = 1 << LayerMask.NameToLayer("DAMAGEABLE");
    }
    private void Update()
    {
        Collider2D col = Physics2D.OverlapCircle(transform.position,Collider.bounds.size.x * 0.5f,_targetLayer);

        if(col != default(Collider2D) && col.TryGetComponent(out PlayerBrain playerBrain))
        {
            Vector3 attackDir = playerBrain.transform.position - transform.position;
            var player =playerBrain.PhotonView.Owner;
            GameManager.Instance.OTCPlayer(player,attackDir);
        }
    }
}
