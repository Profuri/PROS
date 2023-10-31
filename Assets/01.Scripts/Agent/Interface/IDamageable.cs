using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void Damaged(Vector3 originPos,Vector3 direction);
}
