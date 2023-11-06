using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using MonoPlayer;
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }
    
    [SerializeField]
    private PoolingListSO _poolingListSO;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
        //UIManager.Instance.Init();
        
        NetworkManager.Instance.Init();
        SceneManagement.Instance.Init(this.transform);
        PlayerManager.Instance.Init();
        ParticleManager.Instance.Init();
        PoolManager.Instance = new PoolManager(this.transform);

        _poolingListSO.pairs.ForEach(p => PoolManager.Instance.CreatePool(p.prefab,p.count));
    }
}
