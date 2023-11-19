using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseItem : PoolableMono, IItem
{
    [SerializeField] protected ItemSO _itemSO;
    private int _currentHitCnt;
    
    protected Vector2 _moveDir;

    public bool Used { get; set; }

    protected float _movementSpeed;
    protected bool _isSpawnEnd;

    public UnityEvent SpawnEvent;
    public UnityEvent HitEvent;

    public virtual void GenerateSetting(Vector2 moveDir, Vector2 spawnPos, float movementSpeed)
    {
        _movementSpeed = movementSpeed;
        transform.position = spawnPos;
        _moveDir = moveDir;

        Used = false;
        _isSpawnEnd = false;
        _currentHitCnt = 0;

        transform.Find("Visual").GetComponent<SpriteRenderer>().sprite = _itemSO.Sprite;
        SpawnEvent?.Invoke();
    }

    public virtual void UpdateItem()
    {
        if (Used || !_isSpawnEnd)
        {
            return;
        }
        
        transform.Translate(_moveDir * (_movementSpeed * Time.deltaTime), Space.World);
    }

    public virtual bool HitByPlayer(Player hitPlayer)
    {
        _currentHitCnt++;

        if (_currentHitCnt >= _itemSO.UsableHitCnt)
        {
            Used = true;
            OnTakeItem(hitPlayer);
        }
        else
        {
            HitEvent?.Invoke();
        }

        return Used;
    }

    public abstract void OnTakeItem(Player takenPlayer);
}