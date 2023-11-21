using MonoPlayer;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Photon.Pun;

public class DeathMatchStageSystem : BaseStageSystem
{
    [SerializeField] private float _targetTime;

    private Dictionary<Player, int> _killDictionary;
    private Coroutine _coroutine;

    public override void Init(int mapIndex)
    {
        base.Init(mapIndex);

        if(NetworkManager.Instance.IsMasterClient)
        {
            _killDictionary = new Dictionary<Player, int>();

            PlayerManager.Instance.OnAllPlayerLoad += InitKillDictionary; 
            PlayerManager.Instance.OnPlayerAttacked += OnPlayerAttacked;
        }
    }
    
    public override void GenerateNewStage(int index)
    {
        base.GenerateNewStage(index);

    }
    private void InitKillDictionary()
    {
        _killDictionary.Clear();
        foreach (var player in PlayerManager.Instance.LoadedPlayerList)
        {
            if (_killDictionary.ContainsKey(player)) continue;
            _killDictionary.Add(player, 0);
        }

        if(null != _coroutine)
        {
            StopCoroutine(_coroutine);
        }
        _coroutine = StartCoroutine(TimerCor());
    }
    
    public override void StageLeave()
    {
        base.StageLeave();

        if (null != _coroutine)
        {
            StopCoroutine(_coroutine);
        }

        PlayerManager.Instance.OnAllPlayerLoad -= InitKillDictionary;
        PlayerManager.Instance.OnPlayerAttacked -= OnPlayerAttacked;
    }

    public override void RoundCheck(Player player)
    {
        NetworkManager.Instance.PhotonView.RPC(nameof(RoundWinnerRPCDeath), RpcTarget.All,player);
    }

    [PunRPC]
    private void RoundWinnerRPCDeath(Player player)
    {
        RoundWinner(player);
    }

    public void OnPlayerAttacked(Player attacker,Player deadPlayer)
    {
        if(_killDictionary.ContainsKey(attacker) && attacker != null)
        {
            _killDictionary[attacker]++;
        }
    }

    private IEnumerator TimerCor()
    {
        float timer = 0f;
        while(timer <= _targetTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        var bestKiller = default(Player);

        foreach(var kvp in _killDictionary)
        {
            if (bestKiller == null) bestKiller = kvp.Key;
            if (_killDictionary[bestKiller] < kvp.Value)
            {
                bestKiller = kvp.Key;
            }
        }
        RoundCheck(bestKiller);
    }
}
