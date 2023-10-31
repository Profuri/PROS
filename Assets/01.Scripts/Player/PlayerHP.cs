using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
public class PlayerHP : AgentHP
{
    private Coroutine _knockBackCoroutine;
    public override void Damaged(Vector3 originPos,Vector3 direction)
    {
        Debug.Log("Damaged");
        if (_knockBackCoroutine != null)
        {
            StopCoroutine(_knockBackCoroutine);
        }
        _knockBackCoroutine = StartCoroutine(KnockBackCoroutine(direction));
    }

    private IEnumerator KnockBackCoroutine(Vector3 direction)
    {
        yield return new WaitForSeconds(_timeFreezeDelay);
        float timer = 0f;
        while (timer < _knockBackTime)
        {
            timer += Time.deltaTime;
            transform.position += direction * (_knockBackPower * Time.deltaTime);
            yield return null;
        }
    }
}
