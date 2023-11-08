using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine;
public class Score
{
    private Dictionary<Player, int> _winRoundDictionary;
    private int _targetWinCnt;
    public Player IsGameEnd
    {
        get
        {
            foreach (var kvp in _winRoundDictionary)
            {
                if (kvp.Value >= _targetWinCnt)
                {
                    return kvp.Key;
                }
            }
            return default(Player);
        }
    }
    public Score(List<Player> players,int targetWinCnt)
    {
        _winRoundDictionary = new Dictionary<Player, int>();
        foreach (var player in players)
        {
            if (_winRoundDictionary.ContainsKey(player)) continue;
            _winRoundDictionary.Add(player,0);
        }
    }
    public void OnLeftPlayer(Player leftPlayer)
    {
        if (_winRoundDictionary.ContainsKey(leftPlayer))
        {
            _winRoundDictionary.Remove(leftPlayer);
        }
    }

    public void GetScore(Player player)
    {
        ++_winRoundDictionary[player];
        foreach (var kvp in _winRoundDictionary)
        {
            Debug.LogError($"Key: {kvp.Key} Value: {kvp.Value}");
        }
    }
}
