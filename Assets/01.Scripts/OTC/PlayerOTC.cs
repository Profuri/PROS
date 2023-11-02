using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOTC : PlayerHandler
{
    [SerializeField] private float _otcPower = 10f;
    [Range(0f, 1f)]
    [SerializeField] private float _bouncePer;

    /// <summary>
    /// OTC Dropout Execution Function
    /// </summary>
    /// <param name="attackDir"> Attack Object Moving Direction </param>
    /// <param name="otcPrevPos"> Otc Object Previous Moving Direction </param>
    /// <param name="otcCurPos"> Otc Object Current Moving Direction </param>
    public void PlayOTC(Vector3 attackDir)
    {
        Vector3 otcMovingDir = CalcMovingDir(_brain.ActionData.PreviousPos, _brain.ActionData.CurrentPos);      
        Vector3 otcDir = CalcOTCDir(attackDir.normalized, otcMovingDir);

        bool isBounce = false;

        // Cases with OTC Direction X or Y of Zero
        if (otcDir == Vector3.zero)
        {
            otcDir = attackDir;
            isBounce = true;
        }
        if (otcDir != Vector3.zero)
        {
            if (otcDir.x == 0)
            {
                int random = Random.Range(0, 2);
                switch (random)
                {
                    case 0:
                        otcDir.x++;
                        break;
                    case 1:
                        otcDir.x--;
                        break;
                }
            }
            if (otcDir.y == 0)
                otcDir.y++;
        }
        
        // ToDo : There should be a value to check if it has fallen below the floor. (Need Bool Parameter)**
        // Protect flying to the floor
        if (otcDir.y < 0)
            otcDir.y *= -1;

        if (transform.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            if (isBounce)
                rb.AddForce(otcDir * _otcPower, ForceMode2D.Impulse);
            else
                rb.AddForce(otcDir * _otcPower * _bouncePer, ForceMode2D.Impulse);
        }
        else
            Debug.LogError("Otc Object Rigidbody2D not Found");
    }

    private Vector3 CalcOTCDir(Vector3 attackMoveDir, Vector3 otcMoveDir)
    {
        return (attackMoveDir + otcMoveDir).normalized;
    }

    private Vector3 CalcMovingDir(Vector3 prevPos, Vector3 curPos)
    {
        return (curPos - prevPos).normalized;
    }

    public override void BrainUpdate(){}
    public override void BrainFixedUpdate(){}

}
