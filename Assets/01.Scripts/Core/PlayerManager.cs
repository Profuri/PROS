using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Random = UnityEngine.Random;

namespace MonoPlayer
{
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
    
        private List<Player> _playerList;
        
        private Dictionary<Player, PlayerBrain> _brainDictionary;
        public Dictionary<Player, PlayerBrain> BrainDictionary => _brainDictionary;
    
        public event Action GameStartEvent;
        public void Init()
        {
            SceneManagement.Instance.OnGameSceneLoaded += OnGameSceneLoad;
            GameStartEvent += ConvertToDictionary;
    
            _playerList = new List<Player>();
            _brainDictionary = new Dictionary<Player, PlayerBrain>();
        }
    
        private void OnDisable()
        {
            SceneManagement.Instance.OnGameSceneLoaded -= OnGameSceneLoad;
            GameStartEvent -= ConvertToDictionary;
        }
        private void OnGameSceneLoad()
        {
            if (NetworkManager.Instance.IsMasterClient)
            {
                NetworkManager.Instance.PhotonView.RPC("OnGameSceneLoadedRPC", RpcTarget.All);
            }
        }
        
        //각 클라이언트에서 한 번씩 실행됨
        //현재 룸에 있는 플레이어를 전부 등록함
        [PunRPC]
        private void OnGameSceneLoadedRPC()
        {
            float randomX = Random.Range(-5, 5f);
            float y = 5f;
            Vector3 randomPos = new Vector3(randomX,y);

            var prefab = PhotonNetwork.Instantiate("Player",randomPos,Quaternion.identity);
            foreach (Player player in NetworkManager.Instance.PlayerList)
            {
                NetworkManager.Instance.PhotonView.RPC("AddPlayer",RpcTarget.All,player);
            }
        }
    
        //Player가 Add될 때 똑같은 Player 두개가 Add됨
        [PunRPC]
        public void AddPlayer(Player player)
        {
            if (_playerList.Contains(player)) return;

            _playerList.Add(player);
            
            if (_playerList.Count == NetworkManager.Instance.PlayerList.Count)
            {
                GameStartEvent?.Invoke();
            }
        }

        //Add된 플레이어를 바탕으로 현재 게임에있는 플레이어 브레인을 
        //각각에 맞는 플레이어와 로컬플레이어랑 비교해서
        //dictionary에 넣어주는 작업을 해 줌
        private void ConvertToDictionary()
        {
            //두 번 실행되지 않도록 막아줌
            Debug.LogError("ConvertToDictionary");
            List<PlayerBrain> brainList = FindObjectsOfType<PlayerBrain>().ToList();
            
            foreach (var player in _playerList)
            {
                foreach (var brain in brainList)
                {
                    //혹시나 해서 키 같은거 있으면 막아줌
                    if (_brainDictionary.ContainsKey(player)) return;
                    _brainDictionary.Add(player,brain);
                }
            }
            foreach (var a in _brainDictionary)
            {
                Debug.Log($"KVP: {a}");
            }
        }
    }

}

