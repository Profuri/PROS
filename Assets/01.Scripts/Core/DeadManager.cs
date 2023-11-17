using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using MonoPlayer;

public class DeadManager : MonoBehaviour
{
    private static DeadManager _instance;
    public static DeadManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<DeadManager>();
            }
            return _instance;
        }
    }
    [SerializeField] private float _deathDistance = 30f;
    private List<PlayerBrain> _playerBrainList;

    private Coroutine _coroutine;
    public void Init()
    {
        _playerBrainList = new List<PlayerBrain>();
        //SceneManagement.Instance.OnGameSceneLoaded += StartUpdate; 

    }
    public void AddPlayerBrain(Player player)
    {
        NetworkManager.Instance.PhotonView.RPC(nameof(AddPlayerBrainRPC),RpcTarget.All,player);
    }

    public void RemovePlayerBrain(Player player)
    {
        NetworkManager.Instance.PhotonView.RPC(nameof(RemovePlayerBrainRPC),RpcTarget.All,player);
    }

    [PunRPC]
    private void AddPlayerBrainRPC(Player player)
    {
        var brain = PlayerManager.Instance.BrainDictionary[player];
        _playerBrainList.Add(brain);
    }

    [PunRPC]
    private void RemovePlayerBrainRPC(Player player)
    {
        var brain = PlayerManager.Instance.BrainDictionary[player];
        _playerBrainList.Remove(brain);
    }
    
    
    
    private void Update()
    {
        foreach(var brain in _playerBrainList.ToList())
        {
            if(brain == null) _playerBrainList.Remove(brain);
            if(Vector3.Distance(Vector3.zero,brain.transform.position) >= _deathDistance)
            {
                brain.OnPlayerDead();
                _playerBrainList.Remove(brain);
            }
        }
    }
}
