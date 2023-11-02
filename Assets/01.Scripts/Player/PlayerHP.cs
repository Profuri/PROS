using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
public class PlayerHP : PlayerHandler
{
    [SerializeField] private float _knockBackPower;
    [SerializeField] private float _timeFreezeDelay;
    [SerializeField] private float _knockBackTime;
    

    public override void BrainUpdate()
    {
    }

    public override void BrainFixedUpdate()
    {
    }
}
