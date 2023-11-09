using System.Runtime.InteropServices.ComTypes;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private ParticleTypeListSO _particleTypeListSO;

    private Dictionary<EPARTICLE_TYPE,ParticleAgent> _particleDictionary;
    public void Init()
    {
        _particleDictionary = new();

        foreach(var particleType in _particleTypeListSO.particleTypeList)
        {
            _particleDictionary.Add(particleType.eParticle_type,particleType.particle);
        }
    }
    public ParticleAgent GetParticle(EPARTICLE_TYPE eParticle_type) => _particleDictionary[eParticle_type];

}
