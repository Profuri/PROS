using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ParticleAgent : PoolableMono
{
    protected EPARTICLE_TYPE eParticle_type;
    protected ParticleSystem _particleSystem;
    public abstract void PlayerParticle(Vector3 pos);
    public abstract void StopParticle();
}
