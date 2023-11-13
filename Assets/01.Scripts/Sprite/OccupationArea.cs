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
    [SerializeField] private PhotonView _photonView;
    private readonly int _rateHash = Shader.PropertyToID("_ShowRate");
    private readonly int _colorHash = Shader.PropertyToID("_Color");

    private Color _currentColor;
    public void Init()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _percentTxt = transform.Find("PercentTxt").GetComponent<TMP_Text>();
        _photonView = GetComponent<PhotonView>();
        _percentTxt.text = "0%";
    }

    public void SetColor(Color color) =>
        _photonView.RPC("SetColorRPC", RpcTarget.All, color.r, color.g, color.b, color.a);

    [PunRPC]
    private void SetColorRPC(float r,float g,float b,float a)
    {
        _currentColor = new Color(r, g, b, a);
    }
    public void SetValue(float value)
    {
         _photonView.RPC("SetValueRPC",RpcTarget.All,value);
    }
    [PunRPC]
    private void SetValueRPC(float value)
    {
        _spriteRenderer.material.SetFloat(_rateHash,value);
        _spriteRenderer.material.SetColor(_colorHash,_currentColor);
        _percentTxt.text = String.Format("{0:0.0}",value * 100f);
        Debug.Log(_spriteRenderer.material.GetFloat(_rateHash));
    }
}
