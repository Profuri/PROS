using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DashingItem : BaseItem
{
    public override void Init() { }

    public override void OnTakeItem(Player takenPlayer)
    {
        //PlayerManager.Instance.BrainDictionary[takenPlayer].GetComponent<PlayerBuff>().AddBuff(EBuffType.DASHING);
    }

    public override void UpdateItem()
    {
        base.UpdateItem();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _moveDir = Vector2.Reflect(_moveDir, collision.contacts[0].normal);
        Debug.Log("contact");
    }
}
