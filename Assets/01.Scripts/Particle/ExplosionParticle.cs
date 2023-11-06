using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionParticle : ParticleAgent
{
    public override void Init()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    public override void PlayerParticle(Vector3 pos)
    {
        transform.SetPositionAndRotation(pos,Quaternion.identity);
        _particleSystem.Play();
    }

    public override void StopParticle()
    {
        _particleSystem.Stop();
    }
}
