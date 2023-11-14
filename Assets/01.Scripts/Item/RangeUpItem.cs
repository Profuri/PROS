using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeUpItem : BaseItem
{
    public override void Init()
    {

    }
    public override void OnTakeItem(Player takenPlayer)
    {
        //PlayerManager.Instance.BrainDictionary[takenPlayer].GetComponent<PlayerBuff>().AddBuff(EBuffType.RANGEUP);
    }

    public override void GenerateSetting(Vector2 moveDir, Vector2 spawnPos, float movementSpeed)
    {
        base.GenerateSetting(moveDir, spawnPos, movementSpeed);
        _moveDir = Vector2.down;
    }

}
