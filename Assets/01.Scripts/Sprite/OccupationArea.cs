using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccupationArea : PoolableMono
{
    private SpriteRenderer _spriteRenderer;
    private readonly int _rateHash = Shader.PropertyToID("_ShowRate");
    public override void Init()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void SetValue(float value)
    {
        _spriteRenderer.material.SetFloat(_rateHash,value);
        Debug.Log(_spriteRenderer.material.GetFloat(_rateHash));
    }
}
