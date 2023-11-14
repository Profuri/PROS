using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public abstract class BaseEventObject : MonoBehaviourPunCallbacks
{
    protected PhotonView _photonView;
    
    public virtual void Init()
    {
        _photonView = GetComponent<PhotonView>();
    }

    protected abstract void DestroyObject();
}
