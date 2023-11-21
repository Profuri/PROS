using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerColor : PlayerHandler
{
    [SerializeField] private Transform _spriteRenderParent;
    [SerializeField] private TrailRenderer _dashTrail;
    
    private List<SpriteRenderer> _rendererList;
    public Color CurrentColor { get; private set; }
    
    public override void Init(PlayerBrain brain)
    {
        base.Init(brain);
        _rendererList = new List<SpriteRenderer>();
        _spriteRenderParent.GetComponentsInChildren(_rendererList);

    }
    public override void BrainUpdate()
    {
        
    }

    public override void BrainFixedUpdate()
    {
        
    }
    
    public void SetSpriteColor(Color color)
    {
        if(_brain.IsInit == false) return;
        _brain.PhotonView.RPC("SetSpriteColorRPC",RpcTarget.All,color.r,color.g,color.b,color.a);
    }

    [PunRPC]
    private void SetSpriteColorRPC(float r, float g, float b, float a)
    {
        CurrentColor = new Color(r, g, b, a);

        foreach (var spRenderer in _rendererList)
        {
            if(spRenderer != null)
                spRenderer.material.color = CurrentColor;
        }

        _dashTrail.startColor = CurrentColor;
        _dashTrail.endColor = new Color(CurrentColor.r, CurrentColor.g, CurrentColor.b, 0);
    }
}
