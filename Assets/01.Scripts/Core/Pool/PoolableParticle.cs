using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class PoolableParticle : PoolableMono
{
    private ParticleSystem _particleSystem;
    private Coroutine _runningRoutine;
    
    public void SetPositionAndRotation(Vector3 position = default, Quaternion rotation = default)
    {
        _particleSystem.transform.SetPositionAndRotation(position, rotation);    
    }

    public void SetScale(Vector3 scale)
    {
        _particleSystem.transform.localScale = scale;
    }

    public void PlayParticle(float duration = -1)
    {
        if (_runningRoutine != null)
        {
            StopCoroutine(_runningRoutine);
            _runningRoutine = null;
        }

        if(duration <= 0)
        {
            duration = _particleSystem.main.duration;
        }

        _runningRoutine = StartCoroutine(ParticlePlayRoutine(duration));
    }

    private IEnumerator ParticlePlayRoutine(float duration)
    {
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