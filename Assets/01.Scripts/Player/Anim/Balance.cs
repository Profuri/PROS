using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balance : MonoBehaviour
{
    public float TargetRotation;
    public Rigidbody2D Rb;
    public float Force = 500;
    //public float AddForceValue = -40;
    //public float RepressionForceValue = 1;

    private HingeJoint2D _hingeJoint;
    //private Balance _ptBalance;
    //private Rigidbody2D _ptRb;
    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        _hingeJoint = GetComponent<HingeJoint2D>();
    }

    private void Start()
    {
        //_ptBalance = _hingeJoint.connectedBody.GetComponent<Balance>();
        //_ptRb = _ptBalance.GetComponent<Rigidbody2D>();
        //_hingeJoint.useMotor = true;
    }

    public void Update()
    {
        Rb.MoveRotation(Mathf.LerpAngle(Rb.rotation, TargetRotation, Force * Time.deltaTime));
        //float targetRot;
        //TargetRotation = _ptBalance.TargetRotation;
        //targetRot = TargetRotation;

        //if (transform.CompareTag("PowerExcept"))
        //{
        //    targetRot = TargetRotation;
        //}
        //else
        //{
        //    Debug.Log(_ptBalance.TargetRotation);
        //    TargetRotation = _ptBalance.TargetRotation;
        //    targetRot = (_ptBalance.TargetRotation == 0)
        //      ? TargetRotation : TargetRotation + (AddForceValue / RepressionForceValue);
        //}
        // _hingeJoint.motor Mathf.Lerp()
        //Rb.MoveRotation(Mathf.LerpAngle(Rb.rotation
        //    , targetRot, Force * Time.fixedDeltaTime));
    }
}
