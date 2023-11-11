using MonoPlayer;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Scale과 질량을 키워주는 아이템.
public class ScaleUpItem : BaseItem
{
    [SerializeField] private float _skillTime =10f;
    public override void Init()
    {
               
    }

    public override void OnTakeItem(Player takenPlayer)
    {
        PlayerManager.Instance.BrainDictionary[takenPlayer].GetComponent<ItemAbillity>().UseSkill(EItemType.SCALEUP, _skillTime);
    }

    public override void UpdateItem()
    {
        base.UpdateItem();
        
    }

    public override void GenerateSetting(Vector2 moveDir, Vector2 spawnPos, float movementSpeed)
    {
        base.GenerateSetting(moveDir, spawnPos, movementSpeed);
    }
}
