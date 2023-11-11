using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseItem : PoolableMono, IItem
{
    [SerializeField] private int _usableHitCnt;
    private int _currentHitCnt;
    
    private Vector2 _moveDir;

    public bool Used { get; set; }

    private float _movementSpeed;

    public virtual void GenerateSetting(Vector2 moveDir, Vector2 spawnPos, float movementSpeed)
    {
        Used = false;
        _currentHitCnt = 0;
        _movementSpeed = movementSpeed;
        transform.position = spawnPos;
        _moveDir = moveDir;
    }

    public virtual void UpdateItem()
    {
        if (Used)
        {
            return;
        }
        
        transform.Translate(_moveDir * (_movementSpeed * Time.deltaTime), Space.World);
    }

    public virtual bool HitByPlayer(Player hitPlayer)
    {
        _currentHitCnt++;
        
        if (_currentHitCnt >= _usableHitCnt)
        {
            Used = true;
            OnTakeItem(hitPlayer);
        }

        return Used;
    }

    public abstract void OnTakeItem(Player takenPlayer);
}