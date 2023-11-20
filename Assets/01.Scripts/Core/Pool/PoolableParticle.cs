using System.Collections;
using UnityEngine;

public class PoolableParticle : PoolableMono
{
    private ParticleSystem _particleSystem;
    private Coroutine _runningRoutine;
    
    public void SetPositionAndRotation(Vector3 position = default, Quaternion rotation = default)
    {
        _particleSystem.transform.SetPositionAndRotation(position, rotation);    
    }

    public void SetDuration(float duration)
    {
        var main = _particleSystem.main;
        main.duration = duration;
    }

    public void PlayParticle()
    {
        if (_runningRoutine != null)
        {
            StopCoroutine(_runningRoutine);
            _runningRoutine = null;
        }

        _runningRoutine = StartCoroutine(ParticlePlayRoutine());
    }

    private IEnumerator ParticlePlayRoutine()
    {
        var duration = _particleSystem.main.duration;
        const float offset = 0.1f;
        
        _particleSystem.Play();
        yield return new WaitForSeconds(duration + offset);
        _particleSystem.Stop();
        
        PoolManager.Instance.Push(this);
        _runningRoutine = null;
    }

    public override void Init()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _runningRoutine = null;
    }
}