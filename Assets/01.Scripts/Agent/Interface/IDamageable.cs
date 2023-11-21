using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using System;
public interface IDamageable 
{
    public void Damaged(Player attacker,Vector3 attackDirection,bool priority = false);
}
