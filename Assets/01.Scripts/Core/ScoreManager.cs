using System;
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

    public void Init()
    {
        NetworkManager.Instance.OnPlayerLeftRoomEvent += RPCCallRemoveScoreboard;
        SceneManagement.Instance.OnGameSceneLoaded += FindScoreboard;
        SceneManagement.Instance.OnGameSceneLoaded += SettingScoreUI;
    }

    private void Update()
    {
        // test
        if (Input.GetKeyDown(KeyCode.O))
        {
            AddScore();
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
            SceneManagement.Instance.OnGameSceneLoaded -= SettingScoreUI;
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
            _scoreboard.CreateNewEntry(p);
        }
    }
    
    public void AddScore()
    {
        NetworkManager.Instance.LocalPlayer.AddScore(1);
        NetworkManager.Instance.PhotonView.RPC("UpdateScoreboard", RpcTarget.All, NetworkManager.Instance.LocalPlayer);
        
        if (NetworkManager.Instance.LocalPlayer.GetScore() >= 4)
        {
            // win
        }
    }
    
    [PunRPC]
    private void UpdateScoreboard(Player targetPlayer)
    {
        _scoreboard.UpdateScoreboard(targetPlayer);
    }

    private void RPCCallRemoveScoreboard(Player targetPlayer)
    {
        NetworkManager.Instance.PhotonView.RPC("RemoveScoreboard", RpcTarget.All, targetPlayer);
    }
    
    [PunRPC]
    private void RemoveScoreboard(Player targetPlayer)
    {
        _scoreboard.RemoveEntry(targetPlayer);
    }
}