using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Movement")]
public class MovementSO : ScriptableObject
{
    public float Speed;
    public float JumpPower;
    
    [Tooltip("1을 기준으로 곱해질 값, ex) 2 => Vector2(2,2)")]
    public float DashPower;
    
}
