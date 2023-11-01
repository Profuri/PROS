using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
public class PlayerHP : PlayerHandler,IDamageable
{
    [SerializeField] private float _knockBackPower;
    [SerializeField] private float _timeFreezeDelay;
    [SerializeField] private float _knockBackTime;
    
    private Coroutine _knockBackCoroutine;
    public void Damaged(Vector3 originPos,Vector3 direction,Action Callback = null)
    {
        Debug.Log("Damaged");
        if (_knockBackCoroutine != null)
        {
            StopCoroutine(_knockBackCoroutine);
        }
        _knockBackCoroutine = StartCoroutine(KnockBackCoroutine(originPos, direction,Callback));
    }

    private IEnumerator KnockBackCoroutine(Vector3 originPos, Vector3 direction,Action Callback)
    {
        yield return new WaitForSeconds(_timeFreezeDelay);
        //_brain.Collider.enabled = false;
        float timer = 0f;
        Vector3 knockBackDirection = (transform.position - originPos + direction).normalized;
        while (timer < _knockBackTime)
        {
            timer += Time.deltaTime;
            transform.position += knockBackDirection * (_knockBackPower * Time.deltaTime);
            yield return null;
        }
        //_brain.Collider.enabled = true;
        Callback?.Invoke();
    }

    public override void BrainUpdate()
    {
    }

    public override void BrainFixedUpdate()
    {
    }
}
