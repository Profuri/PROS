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
        
        public Dictionary<Player,Color> ColorDictionary { get; private set; }
        public Dictionary<Player,PlayerBrain> BrainDictionary { get; private set; }

        public bool IsAllOfPlayerLoad { get; private set; } = false;
        public event Action OnAllPlayerLoad;

        [SerializeField] private float _reviveTimer;
        private WaitForSeconds _reviveWaitForSeconds;

        [SerializeField] private GameObject _playerObj;

            #region Init
        public void Init()
        {
            ColorDictionary = new Dictionary<Player, Color>();
            
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
            Color color = ColorDictionary[revivePlayer];
            
            NetworkManager.Instance.PhotonView.RPC(nameof(ShowSpriteRPC),RpcTarget.All,randomPos, color.r, color.g, color.b, color.a);
            yield return _reviveWaitForSeconds;
            CreatePlayer(revivePlayer, randomPos);
        }
        
        [PunRPC]
        private void ShowSpriteRPC(Vector3 spawnPos,float r,float g,float b,float a)
        {
            PlayerSprite playerSprite = PoolManager.Instance.Pop("PlayerSprite") as PlayerSprite;
         
            playerSprite.Init();
            playerSprite.transform.position = spawnPos;
            playerSprite.SetColor(new Color(r,g,b,a));
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
            //prefab.GetComponent<PlayerBrain>().Init();

            var localPlayer = NetworkManager.Instance.LocalPlayer;
                        
            Color randomColor = new Color(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), 1f);
            NetworkManager.Instance.PhotonView.RPC("LoadPlayerListRPC", RpcTarget.All, 
                localPlayer,randomColor.r,randomColor.g,randomColor.b,randomColor.a);
        }

        public void RemovePlayer(Player player) =>
            NetworkManager.Instance.PhotonView.RPC("RemovePlayerRPC", RpcTarget.All, player);
        
        [PunRPC]
        private void RemovePlayerRPC(Player player)
        {
            if (BrainDictionary.ContainsKey(player) == false || LoadedPlayerList.Contains(player) == false) return;
            //This stop Coroutines makes error (not revive player because of RPC)
            //StopAllCoroutines();
            var playerBrain = BrainDictionary[player];

            if (playerBrain.PhotonView.IsMine)
            {
                var obj = playerBrain.gameObject;

                LoadedPlayerList.Remove(player);
                //BrainDictionary.Remove(player);
                //ColorDictionary.Remove(player);
                
                //Debug.LogError($"Destroy Player: {player}");
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

                if (ColorDictionary.ContainsKey(player) == false)
                {
                    ColorDictionary.Add(player,new Color(r,g,b,a));
                }
            }
            
            if (LoadedPlayerList.Count == NetworkManager.Instance.PlayerList.Count)
            {
                Debug.Log("OnAllPlayerLoad");
                OnAllPlayerLoad?.Invoke();
                NetworkManager.Instance.PhotonView.RPC(nameof(LoadBrainDictionaryRPC),RpcTarget.All,player);
                IsAllOfPlayerLoad = true;
            }
        }

        [PunRPC]
        private void LoadBrainDictionaryRPC(Player player)
        {
            var players = FindObjectsOfType<PlayerBrain>().ToList();
            PlayerBrain playerBrain = players.Find(p => p.PhotonView.Owner == player);

            if(playerBrain.IsInit == false) return;
            playerBrain.SetName(player.NickName);

            Color color = ColorDictionary[player];
            playerBrain.PlayerColor.SetSpriteColor(color);

            if(BrainDictionary.ContainsKey(player))
            {
                BrainDictionary[player] = playerBrain;
            }
            else
            {
                BrainDictionary.TryAdd(player,playerBrain);
            }
        }
        #endregion
    }
}