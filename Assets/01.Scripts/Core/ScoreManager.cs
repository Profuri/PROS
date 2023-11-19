using System;
using MonoPlayer;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;

public class ScoreManager : MonoBehaviourPunCallbacks
{
    private static ScoreManager _instance;
    public static ScoreManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ScoreManager>();
            }
            return _instance;
        }
    }

    private ScoreboardUI _scoreboard;

    public event Action<Player> OnDecideWinnerEvent = null;

    public void Init()
    {
        NetworkManager.Instance.OnPlayerLeftRoomEvent += RPCCallRemoveScoreboard;
        SceneManagement.Instance.OnGameSceneLoaded += FindScoreboard;
        PlayerManager.Instance.OnAllPlayerLoad += SettingScoreUI;
    }

    private void Update()
    {
        // test
        if (Input.GetKeyDown(KeyCode.O))
        {
            AddScore(NetworkManager.Instance.LocalPlayer);
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();

        if (NetworkManager.Instance)
        {
            NetworkManager.Instance.OnPlayerLeftRoomEvent -= RPCCallRemoveScoreboard;
        }

        if (SceneManagement.Instance)
        {
            SceneManagement.Instance.OnGameSceneLoaded -= FindScoreboard;
        }

        if (PlayerManager.Instance)
        {
            PlayerManager.Instance.OnAllPlayerLoad -= SettingScoreUI;
        }
    }

    private void FindScoreboard()
    {
        _scoreboard = FindObjectOfType<ScoreboardUI>();
    }

    private void SettingScoreUI()
    {
        foreach (var p in NetworkManager.Instance.PlayerList)
        {
            _scoreboard.RemoveEntry(p);
            _scoreboard.CreateNewEntry(p);
        }
    }
    
    public void AddScore(Player targetPlayer)
    {
        targetPlayer.AddScore(1);
        NetworkManager.Instance.PhotonView.RPC("UpdateScoreboard", RpcTarget.All, targetPlayer);
        
        if (targetPlayer.GetScore() >= 4)
        {
            OnDecideWinnerEvent?.Invoke(targetPlayer);
        }
    }
    
    [PunRPC]
    private void UpdateScoreboard(Player targetPlayer)
    {
        _scoreboard.UpdateScoreboard(targetPlayer);
    }

    private void RPCCallRemoveScoreboard(Player targetPlayer)
    {
        if (_scoreboard is null)
        {
            return;
        }
        
        NetworkManager.Instance.PhotonView.RPC("RemoveScoreboard", RpcTarget.All, targetPlayer);
    }
    
    [PunRPC]
    private void RemoveScoreboard(Player targetPlayer)
    {
        _scoreboard.RemoveEntry(targetPlayer);
    }
}