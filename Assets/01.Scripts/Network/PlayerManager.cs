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

        private List<Player> _loadedPlayerList;
        public List<Player> LoadedPlayerList => _loadedPlayerList;
        private Dictionary<Player, PlayerBrain> _brainDictionary;
        public Dictionary<Player, PlayerBrain> BrainDictionary => _brainDictionary;
        public event Action OnAllPlayerLoad;
        #region Init
        public void Init()
        {
            SceneManagement.Instance.OnGameSceneLoaded += OnGameSceneLoad;
            NetworkManager.Instance.OnPlayerLeftRoomEvent += OnLeftPlayer;
            
            OnAllPlayerLoad += ConvertToDictionary;

            _loadedPlayerList = new List<Player>();
            _brainDictionary = new Dictionary<Player, PlayerBrain>();
        }
        private void OnDisable()
        {
            SceneManagement.Instance.OnGameSceneLoaded -= OnGameSceneLoad;
            NetworkManager.Instance.OnPlayerLeftRoomEvent -= OnLeftPlayer;

            OnAllPlayerLoad -= ConvertToDictionary;
        }
        #endregion

        
        private void OnLeftPlayer(Player leftPlayer)
        {
            if (_loadedPlayerList.Contains(leftPlayer))
            {
                _loadedPlayerList.Remove(leftPlayer);
            }
        }
        
        
        #region PlayerDictionarySetting
        private void OnGameSceneLoad()
        {

            
            var prefab = PhotonNetwork.Instantiate("Player",Vector3.zero,Quaternion.identity);
            
            PlayerBrain brain = prefab.GetComponent<PlayerBrain>();
            brain.SetName(NetworkManager.Instance.LocalPlayer.NickName);

            var player = NetworkManager.Instance.LocalPlayer;

            NetworkManager.Instance.PhotonView.RPC("AddPlayerToDictionary",RpcTarget.All,player);
        }
        
        
        [PunRPC]
        private void AddPlayerToDictionary(Player player)
        {
            _loadedPlayerList.Add(player);
            if (_loadedPlayerList.Count == NetworkManager.Instance.PlayerList.Count)
            {
                OnAllPlayerLoad?.Invoke();
            }
        }

        public void SetBrainRandomPos()
        {
            NetworkManager.Instance.PhotonView.RPC("SetBrainRandomPosRPC",RpcTarget.All);   
        }
        [PunRPC]
        private void SetBrainRandomPosRPC()
        {
            foreach (var player in PlayerManager.Instance.LoadedPlayerList)
            {
                float randomX = Random.Range(-5, 5f);
                float y = 5f;
                Vector3 randomPos = new Vector3(randomX,y);

                PlayerBrain brain = PlayerManager.Instance.BrainDictionary[player];
                brain.Init(randomPos);
            }
        }
        

        //Add된 플레이어를 바탕으로 현재 게임에있는 플레이어 브레인을 
        //각각에 맞는 플레이어와 로컬플레이어랑 비교해서
        //dictionary에 넣어주는 작업을 해 줌
        private void ConvertToDictionary()
        {
            //애초에 brain을 제대로 안 찾아와 줌
            var brainList = FindObjectsOfType<PlayerBrain>();
            
            foreach (var player in _loadedPlayerList)
            {
                foreach (var brain in brainList)
                {
                    //혹시나 해서 키 같은거 있으면 막아줌
                    if (_brainDictionary.ContainsKey(player)) return;
                    if (brain.gameObject.name == player.NickName)
                    {
                        _brainDictionary.Add(player,brain);
                    }
                }
            }
        }
        #endregion
        
    }
}

