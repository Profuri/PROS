using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balance : MonoBehaviour
{
    public float TargetRotation;
    public Rigidbody2D Rb;
    public float Force = 500;
    public float RepressionForceValue = 1;

    private HingeJoint2D _hingeJoint;
    private Balance _ptBalance;
    
    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        _hingeJoint = GetComponent<HingeJoint2D>();
    }

    private void Start()
    {
        _ptBalance = _hingeJoint.connectedBody.GetComponent<Balance>();
    }

    public void Update()
    {
        float targetRot;
        if (transform.CompareTag("PowerExcept"))
        {
            targetRot = TargetRotation;
        }
        else
        {
            targetRot = (_ptBalance.TargetRotation == 0)
              ? TargetRotation : TargetRotation + (_ptBalance.TargetRotation / RepressionForceValue);
        }
       
        Rb.MoveRotation(Mathf.LerpAngle(Rb.rotation
            , targetRot, Force * Time.fixedDeltaTime));
    }
}
