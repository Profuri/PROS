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

        [SerializeField] private float _reviveTimer;
        private WaitForSeconds _reviveWaitForSeconds;

        [SerializeField] private GameObject _playerObj;


        public event Action<Player> OnPlayerDead;

            #region Init
        public void Init()
        {
            NetworkManager.Instance.OnPlayerLeftRoomEvent += OnLeftPlayer;
            //SceneManagement.Instance.OnGameSceneLoaded += RoundStart;
            //OnAllPlayerLoad += () => NetworkManager.Instance.PhotonView.RPC("LoadBrainDictionaryRPC",RpcTarget.All,NetworkManager.Instance.LocalPlayer);

            LoadedPlayerList = new List<Player>();
            BrainDictionary = new Dictionary<Player, PlayerBrain>();

            _reviveWaitForSeconds = new WaitForSeconds(_reviveTimer);
        }
        public override void OnDisable()
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
            //Show sprite player will apppear
            Vector3 randomPos = StageManager.Instance.CurStage.GetRandomSpawnPoint();
            
            var r = (float)revivePlayer.CustomProperties["R"];
            var g = (float)revivePlayer.CustomProperties["G"];
            var b = (float)revivePlayer.CustomProperties["B"];
            
            NetworkManager.Instance.PhotonView.RPC(nameof(ShowSpriteRPC),RpcTarget.All,randomPos, r, g, b);
            yield return _reviveWaitForSeconds;
            CreatePlayer(revivePlayer, randomPos);
        }
        
        [PunRPC]
        private void ShowSpriteRPC(Vector3 spawnPos,float r,float g,float b)
        {
            PlayerSprite playerSprite = PoolManager.Instance.Pop("PlayerSprite") as PlayerSprite;
         
            playerSprite.Init();
            playerSprite.transform.position = spawnPos;
            playerSprite.SetColor(new Color(r,g,b));
            playerSprite.SetDestroy(_reviveTimer);            
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

        public void ReadyPlayer(Player player)
        {
            NetworkManager.Instance.PhotonView.RPC(nameof(ReadyPlayerRPC), RpcTarget.All, player);
        }
        
        [PunRPC]
        public void ReadyPlayerRPC(Player player)
        {
            var waitingRoom = UIManager.Instance.TopComponent as WaitingRoomScreen;
            if (waitingRoom is null)
            {
                Debug.Log("Is not waiting other player");
                return;
            }
            waitingRoom.ReadyPlayer(player);
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
                        
            var r = (float)player.CustomProperties["R"];
            var g = (float)player.CustomProperties["G"];
            var b = (float)player.CustomProperties["B"];
            
            NetworkManager.Instance.PhotonView.RPC(nameof(LoadPlayerListRPC), RpcTarget.All, localPlayer, r, g, b, 1f);
            NetworkManager.Instance.PhotonView.RPC(nameof(LoadBrainDictionaryRPC), RpcTarget.All, player);
            
            if (LoadedPlayerList.Count == NetworkManager.Instance.PlayerList.Count)
            {
                Debug.Log("OnAllPlayerLoad");
            }
        }

        public void RemovePlayer(Player player) =>
            NetworkManager.Instance.PhotonView.RPC("RemovePlayerRPC", RpcTarget.All, player);
        
        [PunRPC]
        private void RemovePlayerRPC(Player player)
        {
            if (BrainDictionary.ContainsKey(player) == false || LoadedPlayerList.Contains(player) == false) return;
            //This stop Coroutines makes error (not revive player because of RPC)
            //StopAllCoroutines();

            if(NetworkManager.Instance.IsMasterClient)
            {
                OnPlayerDead?.Invoke(player);
            }
            var playerBrain = BrainDictionary[player];


            LoadedPlayerList.Remove(player);

            if (playerBrain.PhotonView.IsMine)
            {
                var obj = playerBrain.gameObject;

                PhotonNetwork.Destroy(obj);
                if (StageManager.Instance.CurStage.Mode != EStageMode.SURVIVE)
                {
                    RevivePlayer(player);
                }
            }
        }
        
        [PunRPC]
        private void LoadPlayerListRPC(Player player,float r,float g,float b,float a)
        {
            if (!LoadedPlayerList.Contains(player))
            {
                LoadedPlayerList.Add(player);
            }
        }

        [PunRPC]
        private void LoadBrainDictionaryRPC(Player player)
        {
            OnAllPlayerLoad?.Invoke();
            
            var players = FindObjectsOfType<PlayerBrain>().ToList();
            PlayerBrain playerBrain = players.Find(p => p.PhotonView.Owner == player);

            playerBrain.SetName(player.NickName);
            
            var r = (float)player.CustomProperties["R"];
            var g = (float)player.CustomProperties["G"];
            var b = (float)player.CustomProperties["B"];
            var color = new Color(r, g, b, 1);
            playerBrain.PlayerColor.SetSpriteColor(color);

            if(BrainDictionary.ContainsKey(player))
            {
                BrainDictionary[player] = playerBrain;
            }
            else
            {
                BrainDictionary.TryAdd(player,playerBrain);
            }
            
            IsAllOfPlayerLoad = true;
        }
        #endregion
    }
}