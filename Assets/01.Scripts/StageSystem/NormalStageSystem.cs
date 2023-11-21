using MonoPlayer;
using Photon.Realtime;
using UnityEngine;

public class NormalStageSystem : BaseStageSystem
{
    public override void RoundCheck(Player player)
    {
        if (PlayerManager.Instance.LoadedPlayerList.Count == 1)
        {
            var winner = PlayerManager.Instance.LoadedPlayerList[0];
            RoundWinner(winner);
        }
    }
}