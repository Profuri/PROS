using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
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
    public void StartGenerateItem()
    {
        return;
        if (!NetworkManager.Instance.IsMasterClient || _runningRoutine != null)
        {
            return;
        }
        
        _runningRoutine = StartCoroutine(GenerateItemRoutine());
    }

    public void StopGenerateItem()
    {
        return;
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

            //var type = Random.Range(0, (int)EItemType.COUNT);
            var type = (int)EItemType.DOUBLEDASH_ITEM;
            var moveDir = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized;
            var spawnPos = new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
            var moveSpeed = Random.Range(1f, 2f);
            
            GenerateItemRPC(type,moveDir,spawnPos,moveSpeed);
            //NetworkManager.Instance.PhotonView.RPC("GenerateItemRPC", RpcTarget.All, type, moveDir, spawnPos, moveSpeed);
        }
    }

    public void RemoveAllItem()
    {
        for(int i = 0; i < _items.Count; i++)
        {
            RemoveItem(_items[i]);
            //NetworkManager.Instance.PhotonView.RPC("RemoveItemRPC", RpcTarget.All, i);
        }
        Debug.Log("������ ����");
        _items.Clear();
    }

    public void RemoveItem(BaseItem item)
    {
        if(NetworkManager.Instance.IsMasterClient == false) return;
        PhotonNetwork.Destroy(item.gameObject);
        int index = _items.IndexOf(item);
        _items.RemoveAt(index);
    }
    public void GenerateItemRPC(int type, Vector2 moveDir, Vector2 spawnPos, float moveSpeed)
    {
        var prefab = PhotonNetwork.Instantiate($"{(EItemType)type}",spawnPos,Quaternion.identity);
        var item = prefab.GetComponent<BaseItem>();

        item.GenerateSetting(moveDir, spawnPos, moveSpeed);
        _items.Add(item);
    }
}