using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemManager : MonoBehaviourPunCallbacks
{
    private static ItemManager _instance = null;
    public static ItemManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ItemManager>();
            }
            return _instance;
        }
    }

    [SerializeField] private float _minItemSpawnDelay;
    [SerializeField] private float _maxItemSpawnDelay;

    private Coroutine _runningRoutine;

    private readonly List<BaseItem> _items = new List<BaseItem>();

    public void Init()
    {
        _runningRoutine = null;
    }

    private void Update()
    {
        for (var i = 0; i < _items.Count; )
        {
            if (_items[i].Used)
            {
                NetworkManager.Instance.PhotonView.RPC("RemoveItemRPC", RpcTarget.All, i);       
            }
            else
            {
                _items[i++].UpdateItem();
            }
        }
    }

    public void StartGenerateItem()
    {
        if (!NetworkManager.Instance.IsMasterClient || _runningRoutine != null)
        {
            return;
        }
        
        _runningRoutine = StartCoroutine(GenerateItemRoutine());
    }

    public void StopGenerateItem()
    {
        if (!NetworkManager.Instance.IsMasterClient || _runningRoutine == null)
        {
            return;
        }

        StopCoroutine(_runningRoutine);
        _runningRoutine = null;
    }

    private IEnumerator GenerateItemRoutine()
    {
        while (true)
        {
            var delay = Random.Range(_minItemSpawnDelay, _maxItemSpawnDelay);
            yield return new WaitForSeconds(delay);

            var type = Random.Range(0, (int)EItemType.COUNT);
            var moveDir = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized;
            var spawnPos = new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
            var moveSpeed = Random.Range(1f, 2f);
            
            NetworkManager.Instance.PhotonView.RPC("GenerateItemRPC", RpcTarget.All, type, moveDir, spawnPos, moveSpeed);
        }
    }

    [PunRPC]
    public void GenerateItemRPC(int type, Vector2 moveDir, Vector2 spawnPos, float moveSpeed)
    {
        var item = PoolManager.Instance.Pop($"{(EItemType)type}Item") as BaseItem;

        if (item == null)
        {
            return;
        }
        
        item.GenerateSetting(moveDir, spawnPos, moveSpeed);
        _items.Add(item);
    }

    [PunRPC]
    public void RemoveItemRPC(int removeIndex)
    {
        PoolManager.Instance.Push(_items[removeIndex]);
        _items.RemoveAt(removeIndex);
    }
}