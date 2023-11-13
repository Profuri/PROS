using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class OccupationArea : MonoBehaviourPunCallbacks
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private TMP_Text _percentTxt;
    private readonly int _rateHash = Shader.PropertyToID("_ShowRate");
    [SerializeField] private PhotonView _photonView;
    
    public void Init()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _percentTxt = transform.Find("PercentTxt").GetComponent<TMP_Text>();
        _photonView = GetComponent<PhotonView>();
        _percentTxt.text = "0%";
    }
    public void SetValue(float value)
    {
         _photonView.RPC("SetValueRPC",RpcTarget.All,value);
    }
    [PunRPC]
    private void SetValueRPC(float value)
    {
        _spriteRenderer.material.SetFloat(_rateHash,value);
        _percentTxt.text = String.Format("{0:0.0}",value * 100f);
        Debug.Log(_spriteRenderer.material.GetFloat(_rateHash));
    }
}
