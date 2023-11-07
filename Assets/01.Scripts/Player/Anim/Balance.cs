using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balance : MonoBehaviour
{
    public float TargetRotation;
    public Rigidbody2D Rb;
    public float Force = 500;

    private HingeJoint2D _hingeJoint;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        _hingeJoint = GetComponent<HingeJoint2D>();
    }

    public void Update()
    {        
        Rb.MoveRotation(Mathf.LerpAngle(Rb.rotation, TargetRotation, Force * Time.deltaTime));
    }
}
