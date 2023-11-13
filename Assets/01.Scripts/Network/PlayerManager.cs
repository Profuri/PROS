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
        
        public Dictionary<int,Color> ColorDictionary { get; private set; }
        public Dictionary<Player,PlayerBrain> BrainDictionary { get; private set; }

        public bool IsAllOfPlayerLoad { get; private set; } = false;
        public event Action OnAllPlayerLoad;

        [SerializeField] private float _reviveTimer;
        private WaitForSeconds _reviveWaitForSeconds;

        [SerializeField] private GameObject _playerObj;

            #region Init
        public void Init()
        {
            ColorDictionary = new Dictionary<int, Color>();
            
            NetworkManager.Instance.OnPlayerLeftRoomEvent += OnLeftPlayer;
            //SceneManagement.Instance.OnGameSceneLoaded += RoundStart;
            OnAllPlayerLoad += () => NetworkManager.Instance.PhotonView.RPC("LoadBrainDictionaryRPC",RpcTarget.All,NetworkManager.Instance.LocalPlayer);

            LoadedPlayerList = new List<Player>();
            BrainDictionary = new Dictionary<Player, PlayerBrain>();

            _reviveWaitForSeconds = new WaitForSeconds(_reviveTimer);
        }
        
        private void OnDisable()
        {
            NetworkManager.Instance.OnPlayerLeftRoomEvent -= OnLeftPlayer;
        }

        #endregion

        public void RevivePlayer(Player revivePlayer)
        {
            StartCoroutine(ReviveRoutine(revivePlayer));
        }

        private IEnumerator ReviveRoutine(Player revivePlayer)
        {
            yield return _reviveWaitForSeconds;
            CreatePlayer(revivePlayer, StageManager.Instance.CurStage.GetRandomSpawnPoint());
        }

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

        public void RoundStart()
        {
            if (!NetworkManager.Instance.IsMasterClient)
                return;
            
            NetworkManager.Instance.PhotonView.RPC("RoundStartRPC", RpcTarget.All);
        }

        public void RoundEnd()
        {
            if (!NetworkManager.Instance.IsMasterClient)
                return;
            
            NetworkManager.Instance.PhotonView.RPC("RoundEndRPC", RpcTarget.All);
        }
        
        [PunRPC]
        private void RoundStartRPC()
        {
            var randomPos = StageManager.Instance.CurStage.GetRandomSpawnPoint();
            CreatePlayer(NetworkManager.Instance.LocalPlayer,randomPos);
        }

        [PunRPC]
        private void RoundEndRPC()
        {
            ResetPlayer();
        }

        private void ResetPlayer()
        {
            IsAllOfPlayerLoad = false;
            RemovePlayer(NetworkManager.Instance.LocalPlayer);
        }
        
        private void CreatePlayer(Player player,Vector3 spawnPos)
        {
            var prefab = PhotonNetwork.Instantiate(_playerObj.name,spawnPos,Quaternion.identity);
            var localPlayer = NetworkManager.Instance.LocalPlayer;
                        
            NetworkManager.Instance.PhotonView.RPC("LoadPlayerListRPC", RpcTarget.All, localPlayer);
        }

        public void RemovePlayer(Player player) =>
            NetworkManager.Instance.PhotonView.RPC("RemovePlayerRPC", RpcTarget.All, player);
        
        [PunRPC]
        private void RemovePlayerRPC(Player player)
        {
            if (BrainDictionary.ContainsKey(player) == false || LoadedPlayerList.Contains(player) == false) return;

            var playerBrain = BrainDictionary[player];

            if (playerBrain.PhotonView.IsMine)
            {
                var obj = playerBrain.gameObject;
            
                LoadedPlayerList.Remove(player);
                BrainDictionary.Remove(player);
                //ColorDictionary.Remove(player);
            
                PhotonNetwork.Destroy(obj);
                
                if (StageManager.Instance.CurStage.Mode != EStageMode.NORMAL)
                {
                    RevivePlayer(player);
                }
            }
        }
        
        [PunRPC]
        private void LoadPlayerListRPC(Player player)
        {
            if (!LoadedPlayerList.Contains(player))
            {
                LoadedPlayerList.Add(player);
                
                if (ColorDictionary.ContainsKey(player.ActorNumber) == false)
                {
                    if (NetworkManager.Instance.IsMasterClient)
                    {
                        Debug.LogError("ColorDictionaryChanged");
                        Color randomColor = Random.ColorHSV();
                        NetworkManager.Instance.PhotonView.RPC("LoadColorDictionaryRPC",RpcTarget.All,
                            player, randomColor.r,randomColor.g,randomColor.b,randomColor.a);
                    }
                }
            }
            
            if (LoadedPlayerList.Count == NetworkManager.Instance.PlayerList.Count)
            {
                OnAllPlayerLoad?.Invoke();
                IsAllOfPlayerLoad = true;
            }
        }
        
        [PunRPC]
        private void LoadColorDictionaryRPC(Player player,float r, float g, float b, float a)
        {
            ColorDictionary[player.ActorNumber] = new Color(r, g, b, a);
        }
        
        [PunRPC]
        private void LoadBrainDictionaryRPC(Player player)
        {
            var players = FindObjectsOfType<PlayerBrain>().ToList();
            PlayerBrain playerBrain = players.Find(p => p.PhotonView.Owner == player);
            playerBrain.SetName(player.NickName);
            
            bool result = BrainDictionary.TryAdd(player, playerBrain);
            if (result)
            {
                Color color = ColorDictionary[player.ActorNumber];
                BrainDictionary[player].PlayerColor.SetSpriteColor(color);
            }
            else
            {
                Debug.Log($"BrainDictionary has a same key: {player}");
            }
            
        }
        #endregion
    }
}