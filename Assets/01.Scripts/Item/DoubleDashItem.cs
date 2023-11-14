using MonoPlayer;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleDashItem : BaseItem
{
    Rigidbody2D _rb;

    private float t;
    private float _spawnEvtTime = 1f;
    private bool _isTouchingGround = false;

    public override void Init()
    {
        if (_rb == null)
            _rb = GetComponent<Rigidbody2D>();
    }
    public override void OnTakeItem(Player takenPlayer)
    {
        //PlayerManager.Instance.BrainDictionary[takenPlayer].GetComponent<PlayerBuff>().AddBuff(EBuffType.DOUBLEDASH);
    }

    public override void GenerateSetting(Vector2 moveDir, Vector2 spawnPos, float movementSpeed)
    {
        base.GenerateSetting(moveDir, spawnPos, movementSpeed);
        _moveDir = Vector2.down;
        _rb.gravityScale = 0f;
        _isTouchingGround = false;
        t = 0;
    }

    public override void UpdateItem()
    {
        if (_isTouchingGround) return;

        t += Time.deltaTime;
        if (t > _spawnEvtTime)
        {
            _rb.gravityScale = 9.8f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.IsTouchingLayers(LayerMask.GetMask("WALL", "GROUND")))
        {
            _rb.gravityScale = 0f;
            _isTouchingGround = true;
        }
    }

}
