using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Random = UnityEngine.Random;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    private static PlayerManager _instance;

    public static PlayerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerManager>();
            }

            return _instance;
        }
    }

    private int _currentPlayerCnt = 0;
    private List<Player> _playerList;
    
    private Dictionary<Player, PlayerBrain> _brainDictionary;
    public Dictionary<Player, PlayerBrain> BrainDictionary => _brainDictionary;

    public event Action GameStartEvent;

    private event Action _PlayerAddEvent;
    public void Init()
    {
        SceneManagement.Instance.OnGameSceneLoaded += OnGameSceneLoad;
        GameStartEvent += ConvertToDictionary;
        _PlayerAddEvent += ReadyToGameStart;

        _currentPlayerCnt = 0;
        _playerList = new List<Player>();
        _brainDictionary = new Dictionary<Player, PlayerBrain>();
    }

    private void OnDisable()
    {
        SceneManagement.Instance.OnGameSceneLoaded -= OnGameSceneLoad;
        GameStartEvent -= ConvertToDictionary;
        _PlayerAddEvent -= ReadyToGameStart;
    }
    
    private void OnGameSceneLoad()
    {
        if (NetworkManager.Instance.PhotonView.IsMine)
        {
            float randomX = Random.Range(-5, 5f);
            float y = 5f;
            Vector3 randomPos = new Vector3(randomX,y);
            Player localPlayer = NetworkManager.Instance.LocalPlayer;
            var prefab = PhotonNetwork.Instantiate("Player",randomPos,Quaternion.identity);
        
            NetworkManager.Instance.PhotonView.RPC("AddPlayer",RpcTarget.All,localPlayer);
        }
    }

    //Player가 Add될 때 똑같은 Player 두개가 Add됨
    [PunRPC]
    public void AddPlayer(Player player)
    {
        _playerList.Add(player);
        _currentPlayerCnt++;
        _PlayerAddEvent?.Invoke();
    }
    private void ReadyToGameStart()
    {
        if (_currentPlayerCnt == _playerList.Count)
        {
            GameStartEvent?.Invoke();
        }
    }
    
    //RPC로 AddPlayer를 하다보니 ConverToDictionary는 두 번 실행됨
    private void ConvertToDictionary()
    {
        //두 번 실행되지 않도록 막아줌
        if (_brainDictionary.Count == _currentPlayerCnt) return;
        List<PlayerBrain> brainList = FindObjectsOfType<PlayerBrain>().ToList();
        
        
        foreach (var player in _playerList)
        {
            Debug.Log($"SavedPlayer: {player}");
            foreach (var brain in brainList)
            {
                //혹시나 해서 키 같은거 있으면 막아줌
                if (_brainDictionary.ContainsKey(player)) return;
                if ((Player)brain.PhotonView.Owner == player)
                {
                    _brainDictionary.Add(player,brain);
                }
            }

        }
        foreach (var a in _brainDictionary)
        {
            Debug.Log($"KVP: {a}");
        }
    }
}
