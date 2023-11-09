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

        public List<Player> LoadedPlayerList { get; private set; }
        public Dictionary<Player,PlayerBrain> BrainDictionary { get; private set; }

        public bool IsAllOfPlayerLoad { get; private set; } = false;
        public event Action OnAllPlayerLoad;
        
        #region Init
        public void Init()
        {
            SceneManagement.Instance.OnGameSceneLoaded += OnGameSceneLoad;
            NetworkManager.Instance.OnPlayerLeftRoomEvent += OnLeftPlayer;
            OnAllPlayerLoad += () => NetworkManager.Instance.PhotonView.RPC("LoadBrainDictionaryRPC",RpcTarget.All,NetworkManager.Instance.LocalPlayer);

            
            LoadedPlayerList = new List<Player>();
            BrainDictionary = new Dictionary<Player, PlayerBrain>();
        }
        
        private void OnDisable()
        {
            SceneManagement.Instance.OnGameSceneLoaded -= OnGameSceneLoad;
            NetworkManager.Instance.OnPlayerLeftRoomEvent -= OnLeftPlayer;
            
        }

        #endregion

        
        private void OnLeftPlayer(Player leftPlayer)
        {
            if (LoadedPlayerList.Contains(leftPlayer))
            {
                LoadedPlayerList.Remove(leftPlayer);
            }
            if (BrainDictionary.ContainsKey(leftPlayer))
            {
                BrainDictionary.Remove(leftPlayer);
            }
        }
        
        
        #region PlayerDictionarySetting

        public void RoundRestart()
        {
            if (!NetworkManager.Instance.IsMasterClient)
                return;
            
            NetworkManager.Instance.PhotonView.RPC("RoundRestartRPC", RpcTarget.All);
        }
        
        [PunRPC]
        private void RoundRestartRPC()
        {
            ResetPlayer();
            var randomPos = new Vector3(Random.Range(-5f,5f),0f,0f);
            CreatePlayer(NetworkManager.Instance.LocalPlayer,randomPos);
        }

        private void OnGameSceneLoad()
        {
            RoundRestart();
        }

        private void ResetPlayer()
        {
            IsAllOfPlayerLoad = false;
            RemovePlayer(NetworkManager.Instance.LocalPlayer);
        }
        
        public void CreatePlayer(Player player,Vector3 spawnPos)
        {
            var prefab = PhotonNetwork.Instantiate("Player",spawnPos,Quaternion.identity);
            var localPlayer = NetworkManager.Instance.LocalPlayer;
                        
            NetworkManager.Instance.PhotonView.RPC("LoadPlayerListRPC",RpcTarget.All,localPlayer);
        }
        
        public void RemovePlayer(Player player)
        {
            if (BrainDictionary.ContainsKey(player) == false || LoadedPlayerList.Contains(player) == false) return;

            var playerBrain = BrainDictionary[player];

            if (playerBrain.PhotonView.IsMine)
            {
                var obj = playerBrain.gameObject;
            
                LoadedPlayerList.Remove(player);
                BrainDictionary.Remove(player);
            
                PhotonNetwork.Destroy(obj);
            }
        }
                
        [PunRPC]
        private void LoadPlayerListRPC(Player player)
        {
            if (!LoadedPlayerList.Contains(player))
            {
                LoadedPlayerList.Add(player);
            }

            if (LoadedPlayerList.Count == NetworkManager.Instance.PlayerList.Count)
            {
                OnAllPlayerLoad?.Invoke();
                IsAllOfPlayerLoad = true;
            }
        }
        
        [PunRPC]
        private void LoadBrainDictionaryRPC(Player player)
        {
            var players = FindObjectsOfType<PlayerBrain>().ToList();
            PlayerBrain playerBrain = players.Find(p => p.PhotonView.Owner == player);
            playerBrain.SetName(player.NickName);
            
     
            bool result = BrainDictionary.TryAdd(player, playerBrain);
            if (result == false)
            {
                Debug.Log($"BrainDictionary has a same key: {player}");
            }
            
        }
        #endregion
    }
}

