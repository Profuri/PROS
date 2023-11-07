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
        

        //Add???λ ?΄μ΄λ₯?λ°ν?Όλ‘ ?μ¬ κ²μ?μ???λ ?΄μ΄ λΈλ ?Έμ.
        //κ°κ°??λ§λ ?λ ?΄μ΄? λ‘μ»¬?λ ?΄μ΄??λΉκ΅?΄μ.
        //dictionary???£μ΄μ£Όλ ?μ????μ€?.
        private void ConvertToDictionary()
        {
            //? μ΄??brain???λ?λ‘???μ°Ύμ? μ€?
            var brainList = FindObjectsOfType<PlayerBrain>();
            
            foreach (var player in _playerDictionary.Keys)
            {
                foreach (var brain in brainList)
                {
                    //?Ήμ???΄μ ??κ°μ?κ±??μΌλ©?λ§μμ€?.
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

