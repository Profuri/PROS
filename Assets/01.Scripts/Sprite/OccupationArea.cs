using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccupationArea : PoolableMono
{
    private SpriteRenderer _spriteRenderer;
    private Material _mat;
    public override void Init()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _mat = _spriteRenderer.material;
    }
    public void SetValue(float value)
    {
        _mat.SetFloat("_ShowRate",value);
    }
}
