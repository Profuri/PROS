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

    public void PlayParticleAll(string particleName, Vector3 position = default, Vector3 scale = default, Quaternion rotation = default)
    {
        if (scale == default)
        {
            scale = Vector3.one;
        }
        NetworkManager.Instance.PhotonView.RPC("PlayParticleRPC", RpcTarget.All, particleName, position, scale, rotation);
    }
    
    [PunRPC]
    public void PlayParticleRPC(string particleName, Vector3 position, Vector3 scale, Quaternion rotation)
    {
        var particle = PoolManager.Instance.Pop(particleName) as PoolableParticle;

        if (particle == null)
        {
            return;
        }
        
        particle.SetPositionAndRotation(position, rotation);
        particle.SetScale(scale);
        particle.PlayParticle();
    }
}
