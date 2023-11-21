using MonoPlayer;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleDashItem : BaseItem
{
    //Rigidbody2D _rb;

    //private float t;
    //private float _spawnEvtTime = 1f;
    private bool _isTouchingGround;

    public override void Init()
    {
        //if (_rb == null)
        //    _rb = GetComponent<Rigidbody2D>();
        _isTouchingGround = false;
    }
    public override void OnTakeItem(Player takenPlayer)
    {
        PlayerManager.Instance.BrainDictionary[takenPlayer].GetComponent<PlayerBuff>().AddBuff(EBuffType.DOUBLEDASH);
    }

    public override void GenerateSetting(Vector2 moveDir, Vector2 spawnPos, float movementSpeed)
    {
        base.GenerateSetting(moveDir, spawnPos, movementSpeed);
        _moveDir = Vector2.down;
        _isTouchingGround = false;
    }

    public override void UpdateItem()
    {
        if (_isTouchingGround)
        {
            _moveDir = Vector2.zero;
            return;
        }
        base.UpdateItem();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.IsTouchingLayers(LayerMask.GetMask("WALL", "GROUND")))
        {
            _isTouchingGround = true;
        }
    }

}
