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

    public void PlayParticleAll(string particleName, Vector3 position = default, Quaternion rotation = default, float duration = -1f)
    {
        NetworkManager.Instance.PhotonView.RPC("PlayParticleRPC", RpcTarget.All, particleName, position, rotation, duration);
    }
    
    [PunRPC]
    public void PlayParticleRPC(string particleName, Vector3 position, Quaternion rotation, float duration)
    {
        var particle = PoolManager.Instance.Pop(particleName) as PoolableParticle;

        if (particle == null)
        {
            return;
        }
        
        particle.SetPositionAndRotation(position, rotation);

        if (duration >= 0f)
        {
            particle.SetDuration(duration);
        }
        
        particle.PlayParticle();
    }
}
