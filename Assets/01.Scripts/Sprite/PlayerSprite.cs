using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerSprite : PoolableMono
{
    private SpriteRenderer[] _renderers;
    [SerializeField] private float _moveDistance = 1f;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _alphaChangeSpeed = 5f;

    private Color _currentColor = Color.white;
    public override void Init()
    {
        Transform spriteParent = transform.Find("Visual/Sprites");

        _renderers = new SpriteRenderer[spriteParent.childCount];
        _renderers = spriteParent.GetComponentsInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        transform.position += new Vector3(0f,Mathf.Sin(Time.time * _moveSpeed) * _moveDistance ,0f);
        
        foreach (var renderer in _renderers)
        {
            renderer.material.color = new Color(_currentColor.r, _currentColor.g, _currentColor.b, (Mathf.Sin(Time.time * _alphaChangeSpeed) + 1) * 0.5f);
        }
    }

    public void SetColor(Color color)
    {
        _currentColor = color;
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
