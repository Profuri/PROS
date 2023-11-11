using Photon.Realtime;
using UnityEngine;

public interface IItem
{
    public void GenerateSetting(Vector2 moveDir, Vector2 spawnPos, float movementSpeed);
    public void UpdateItem();
    public bool HitByPlayer(Player hitPlayer);
    public void OnTakeItem(Player takenPlayer);
}