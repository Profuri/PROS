using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDefend : PlayerHandler
{
    [SerializeField] private float _coolTime = 2f;
    [SerializeField] private float _defendTime = 1.5f;

    private Coroutine _defendCoroutine;
    private float _prevTime;

    private bool _isDefend;
    public bool IsDefend => _isDefend;
  
    public override void Init(PlayerBrain brain)
    {
        base.Init(brain);
        _prevTime = Time.time;
    }

    public override void BrainFixedUpdate(){}
    public override void BrainUpdate()
    {
        // will be added to the input system
        if (Input.GetKeyDown(KeyCode.F))
        {
            PlayDefense();
        }
    }

    public void PlayDefense()
    {
        if (_prevTime + _coolTime >= Time.time) return;
        if (_defendCoroutine != null)
        {
            StopCoroutine(_defendCoroutine);
        }

        _isDefend = true;
        _prevTime = Time.time;
        _defendCoroutine = StartCoroutine(DefendCoroutine());
    }

    IEnumerator DefendCoroutine()
    {        
        yield return new WaitForSeconds(_defendTime);
        _isDefend = false;
    }
}
