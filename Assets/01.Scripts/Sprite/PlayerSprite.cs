using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSprite : PoolableMono
{
    private SpriteRenderer[] _renderers;
    public override void Init()
    {
        Transform spriteParent = transform.Find("Visual/Sprites");

        _renderers = new SpriteRenderer[spriteParent.childCount];
        _renderers = spriteParent.GetComponentsInChildren<SpriteRenderer>();
    }

    public void SetColor(Color color)
    {
        foreach (var renderer in _renderers)
        {
            renderer.material.color = color;
        }
    }

    public void SetDestroy(float time)
    {
        StartCoroutine(DestroyCor(time));
    }

    private IEnumerator DestroyCor(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }
}
