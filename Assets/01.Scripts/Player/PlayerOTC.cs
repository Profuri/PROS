using UnityEngine;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using UnityEngine.Events;
using System.Collections;

public class PlayerOTC : PlayerHandler, IDamageable
{
    [SerializeField] private ParticleSystem _smokeParticle;
    [SerializeField] private UnityEvent _OnPlayerOTCEvent;
    
    [SerializeField] private float _otcPower;
    [Range(0f, 1f)]
    [SerializeField] private float _bouncePer;

    public override void Init(PlayerBrain brain)
    {
        base.Init(brain);
        //_brain.PlayerBuff.Heavy += Heavy;
    }

    public void Damaged(Vector3 attackDirection, bool priority = false)
    {
        if (_brain.PlayerDefend.IsDefend || _brain.ActionData.IsFlying) return;
        
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
        
        if (_brain.PlayerDefend.IsDefendBounce && priority == false)
        {
            _brain.PlayerDefend.IsDefendBounce = false;
            isBounce = true;
            otcDir = attackDirection.normalized;
        }
        
        PlaySmokeParticle(otcDir);

        if (isBounce)
        {
            _brain.Rigidbody.AddForce(otcDir * (_otcPower * _bouncePer), ForceMode2D.Impulse);
        }
        else
        {
            _OnPlayerOTCEvent?.Invoke();
            _brain.Collider.isTrigger = true;
            _brain.ActionData.IsFlying = true;
            _brain.OnOTC?.Invoke();
            _brain.Rigidbody.AddForce(otcDir * _otcPower, ForceMode2D.Impulse);
        }
    }

    private void PlaySmokeParticle(Vector3 otcDir)
    {
            if(_smokeParticle.isPlaying)
            {
                _smokeParticle.Stop();
            }
            var rotation = Quaternion.LookRotation(-otcDir);
            _smokeParticle.transform.rotation = rotation;
            _smokeParticle.Play();
    }

    private Vector3 CalcOTCDir(Vector3 attackMoveDir, Vector3 otcMoveDir)
    {
        return (attackMoveDir + otcMoveDir).normalized;
    }

    private Vector3 CalcMovingDir(Vector3 prevPos, Vector3 curPos)
    {
        return (curPos - prevPos).normalized;
    }

    private void Heavy(float value, float time)
    {
        if (_brain.IsMine)
            StartCoroutine(HeavyTime(value, time));
    }

    private IEnumerator HeavyTime(float value, float time)
    {
        _otcPower = value;
        yield return new WaitForSeconds(time);
        if (_brain.PlayerBuff.CurrentBuff.HasFlag(EBuffType.HEAVY))
            _brain.PlayerBuff.RevertBuff(EBuffType.HEAVY);
    }

    public override void BrainUpdate(){}
    public override void BrainFixedUpdate(){}
}
