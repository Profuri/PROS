using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class AgentHP : MonoBehaviour,IDamageable
{
    [SerializeField] protected float _knockBackPower;
    [SerializeField] protected float _timeFreezeDelay;
    [SerializeField] protected float _knockBackTime;
    public abstract void Damaged(Vector3 originPos,Vector3 direction);
}
