using System.Runtime.InteropServices.ComTypes;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    private static ParticleManager _instance;
    public static ParticleManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<ParticleManager>();
            }
            return _instance;
        }
    }

    public void PlayParticleAll(string particleName, Vector3 position = default, Quaternion rotation = default)
    {
        NetworkManager.Instance.PhotonView.RPC("PlayParticlePRC", RpcTarget.All, particleName, position, rotation);
    }
    
    [PunRPC]
    public void PlayParticleRPC(string particleName, Vector3 position, Quaternion rotation)
    {
        var particle = PoolManager.Instance.Pop(particleName) as PoolableParticle;

        if (particle == null)
        {
            return;
        }
        
        particle.SetPositionAndRotation(position, rotation);
        particle.PlayParticle();
    }
}
