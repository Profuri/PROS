using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Particle/TypeList")]
public class ParticleTypeListSO : ScriptableObject
{
    public List<ParticleType> particleTypeList = new List<ParticleType>();
}

[System.Serializable]
public enum EPARTICLE_TYPE
{
    NONE = 0, EXPLOSION = 1,
}
[System.Serializable]
public class ParticleType
{
    public EPARTICLE_TYPE eParticle_type;
    public ParticleAgent particle;
}