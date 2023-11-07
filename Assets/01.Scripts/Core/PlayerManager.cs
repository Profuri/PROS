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
    
        private Dictionary<Player,bool> _playerDictionary;
        
        private Dictionary<Player, PlayerBrain> _brainDictionary;
        public Dictionary<Player, PlayerBrain> BrainDictionary => _brainDictionary;
        public event Action GameStartEvent;
        public void Init()
        {
            SceneManagement.Instance.OnGameSceneLoaded += OnGameSceneLoad;
            GameStartEvent += ConvertToDictionary;

            _playerDictionary = new Dictionary<Player, bool>();
            _brainDictionary = new Dictionary<Player, PlayerBrain>();
        }
    
        public override void OnDisable()
        {
            base.OnDisable();
            SceneManagement.Instance.OnGameSceneLoaded -= OnGameSceneLoad;
            GameStartEvent -= ConvertToDictionary;
        }
        
        #region PlayerDictionarySetting
        private void OnGameSceneLoad()
        {
            float randomX = Random.Range(-5, 5f);
            float y = 5f;
            Vector3 randomPos = new Vector3(randomX,y);
            
            var prefab = PhotonNetwork.Instantiate("Player",randomPos,Quaternion.identity);
            
            PlayerBrain brain = prefab.GetComponent<PlayerBrain>();
            brain.SetName(NetworkManager.Instance.LocalPlayer.NickName);

            var player = NetworkManager.Instance.LocalPlayer;
            NetworkManager.Instance.PhotonView.RPC("AddPlayerToDictionary",RpcTarget.All,player,true);
        }
        
        
        [PunRPC]
        private void AddPlayerToDictionary(Player player, bool value)
        {
            _playerDictionary.Add(player,value);
            if (_playerDictionary.Count == NetworkManager.Instance.PlayerList.Count)
            {
                GameStartEvent?.Invoke();
            }
        }
        

        //Add???�레?�어�?바탕?�로 ?�재 게임?�있???�레?�어 브레?�을.
        //각각??맞는 ?�레?�어?� 로컬?�레?�어??비교?�서.
        //dictionary???�어주는 ?�업????�?.
        private void ConvertToDictionary()
        {
            //?�초??brain???��?�???찾아?� �?
            var brainList = FindObjectsOfType<PlayerBrain>();
            
            foreach (var player in _playerDictionary.Keys)
            {
                foreach (var brain in brainList)
                {
                    //?�시???�서 ??같�?�??�으�?막아�?.
                    if (_brainDictionary.ContainsKey(player)) return;

                    
                    if (brain.gameObject.name == player.NickName)
                    {
                        _brainDictionary.Add(player,brain);
                    }
                }
            }
        }
        #endregion
        
        #region OTCSystem
        
        public void OTCPlayer(Player player,Vector3 attackDir)
        {
            BrainDictionary[player].PlayerOTC.Damaged(attackDir);
        }
        #endregion
    }
}

