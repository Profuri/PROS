using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;

public abstract class BaseItem : PoolableMono, IItem
{
    [SerializeField] protected ItemSO _itemSO;
    private int _currentHitCnt;
    
    protected Vector2 _moveDir;


    protected float _movementSpeed;
    protected bool _isSpawnEnd;
    protected float _spawnT;

    protected PhotonView _photonView;
    public UnityEvent SpawnEvent;
    public UnityEvent HitEvent;

    public virtual void GenerateSetting(Vector2 moveDir, Vector2 spawnPos, float movementSpeed)
    {
        _movementSpeed = movementSpeed;
        transform.position = spawnPos;
        _moveDir = moveDir;

        _isSpawnEnd = false;
        _currentHitCnt = 0;
        _spawnT = 0;

        _photonView = GetComponent<PhotonView>();
        SpawnEvent?.Invoke();
    }

    public virtual void UpdateItem()
    {
        _spawnT += Time.deltaTime;
        if(_spawnT > 1) _isSpawnEnd = true;

        //if (Used || !_isSpawnEnd)
        //{
        //    return;
        //}
        
        transform.Translate(_moveDir * (_movementSpeed * Time.deltaTime), Space.World);
    }

    public virtual bool HitByPlayer(Player hitPlayer)
    {
        _currentHitCnt++;

        Debug.Log("ItemHit");
        if (_currentHitCnt >= _itemSO.UsableHitCnt)
        {
            ItemManager.Instance.RemoveItem(this);
            OnTakeItem(hitPlayer);
        }
        else
        {
            HitEvent?.Invoke();
        }
        return true;
    }

    public abstract void OnTakeItem(Player takenPlayer);
}