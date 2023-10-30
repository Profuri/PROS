using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : PlayerHandler
{
    public override void Init(PlayerBrain brain)
    {
        base.Init(brain);
        _brain.InputSO.OnDashKeyPress += Dash;
    }

    private void Dash()
    {
        StartCoroutine(DashCoroutine(dashTime: 0.15f,power: 5f));
    }

    private IEnumerator DashCoroutine(float dashTime,float power)
    {
        float timer = 0f;
        while (timer < dashTime)
        {
            timer += Time.deltaTime;
            transform.position += Vector3.zero;
            yield return null;
        }
    }

    public override void BrainUpdate()
    {
    }

    public override void BrainFixedUpdate()
    {
    }
}
