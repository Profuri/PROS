using UnityEngine;
using Photon.Pun;

public class PlayerOTC : PlayerHandler, IDamageable
{
    [SerializeField] private float _otcPower;
    [Range(0f, 1f)]
    [SerializeField] private float _bouncePer;

    public void Damaged(Vector3 attackDirection)
    {
        if (_brain.PlayerDefend.IsDefend) return;

        //_brain.SetColsTrigger(false);
        
        Vector3 otcMovingDir = CalcMovingDir(_brain.ActionData.PreviousPos, _brain.ActionData.CurrentPos);
        Vector3 otcDir = CalcOTCDir(attackDirection.normalized, otcMovingDir);

        bool isBounce = false;
        if (!_brain.ActionData.IsDashing)
        {
            otcDir = attackDirection;
        }
        else
        {
            // Cases with OTC Direction X or Y of Zero
            if (otcDir == Vector3.zero)
            {
                otcDir = attackDirection;
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
        }

        // ToDo : There should be a value to check if it has fallen below the floor. (Need Bool Parameter).
        // Protect flying to the floor
        if (otcDir.y < 0)
            otcDir.y *= -1;
        
        otcDir.Normalize();

        if (_brain.PlayerDefend.IsDefendBounce)
        {
            _brain.PlayerDefend.IsDefendBounce = false;
            isBounce = true;
            otcDir = attackDirection.normalized;
        }

        if (isBounce)
        {
            _brain.Rigidbody.AddForce(otcDir * (_otcPower * _bouncePer), ForceMode2D.Impulse);
        }
        else
        {
            Debug.Log(otcDir);
            // _brain.Collider.enabled = false;
            _brain.Rigidbody.AddForce(otcDir * _otcPower, ForceMode2D.Impulse);
        }
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
