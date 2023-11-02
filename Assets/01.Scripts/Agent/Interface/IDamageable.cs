using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public interface IDamageable
{
    public void Damaged(Transform attackerTrm ,Vector3 attackDirection,Action Callback = null);
}
