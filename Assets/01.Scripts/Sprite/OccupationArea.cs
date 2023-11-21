using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class OccupationArea : MonoBehaviourPunCallbacks
{
    private SpriteRenderer _spriteRenderer;
    private TMP_Text _percentTxt;
    private PhotonView _photonView;
    
    public SpriteRenderer SpriteRenderer
    {
        get
        {
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }
            return _spriteRenderer;
        }
    }
    public TMP_Text PercentTxt
    {
        get
        {
            if (_percentTxt == null)
            {
                _percentTxt = GetComponentInChildren<TMP_Text>();
            }
            return _percentTxt;
        }
    }
    public PhotonView PhotonView
    {
        get
        {
            if (_photonView == null)
            {
                _photonView = GetComponent<PhotonView>();
            }
            return _photonView;
        }
    }
        
    private readonly int _rateHash = Shader.PropertyToID("_ShowRate");
    private readonly int _colorHash = Shader.PropertyToID("_Color");

    private Color _currentColor;

    public void Init()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _photonView = GetComponent<PhotonView>();
        _percentTxt = GetComponent<TMP_Text>();
        PercentTxt.text = "0%";
    }

    public void SetColor(Color color) =>
        PhotonView.RPC("SetColorRPC", RpcTarget.All, color.r, color.g, color.b, color.a);

    [PunRPC]
    private void SetColorRPC(float r,float g,float b,float a)
    {
        _currentColor = new Color(r, g, b, a);
    }
    public void SetValue(float value)
    {
        PhotonView.RPC("SetValueRPC",RpcTarget.All,value);
    }
    
    [PunRPC]
    private void SetValueRPC(float value)
    {
        float setValue = value;
        if(value >= 1f)
        {
            setValue = 1f;
        }
        SpriteRenderer.material.SetFloat(_rateHash, setValue);
        SpriteRenderer.material.SetColor(_colorHash,_currentColor);
        PercentTxt.text = String.Format("{0:0.0}", setValue * 100f);
        //Debug.Log(SpriteRenderer.material.GetFloat(_rateHash));
    }
}
