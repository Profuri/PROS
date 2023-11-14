using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionData : MonoBehaviour
{
    public bool IsDashing;
    public bool IsJumping;
    public bool IsLanding;
    public bool IsFlying;
    public Vector3 PreviousPos;
    public Vector3 CurrentPos;
}
