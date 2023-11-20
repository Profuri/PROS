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

    public void Init()
    {
        NetworkManager.Instance.OnPlayerLeftRoomEvent += RPCCallRemoveScoreboard;
        SceneManagement.Instance.OnGameSceneLoaded += FindScoreboard;
        PlayerManager.Instance.OnAllPlayerLoad += SettingScoreUI;
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
    
    public bool AddScore(Player targetPlayer)
    {
        var score = (int)targetPlayer.CustomProperties["Score"];
        targetPlayer.CustomProperties["Score"] = ++score;
        _scoreboard.UpdateScoreboard(targetPlayer);

        if (score < 4)
        {
            return false;
        }
        
        StageManager.Instance.StageWinner(targetPlayer);
        return true;

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