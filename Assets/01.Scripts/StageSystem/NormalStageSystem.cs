using MonoPlayer;
using Photon.Realtime;
using UnityEngine;

public class NormalStageSystem : BaseStageSystem
{
    public override bool RoundCheck(out Player roundWinner)
    {
        roundWinner = null;
        if (PlayerManager.Instance.LoadedPlayerList.Count == 1)
        {
            var winner = PlayerManager.Instance.LoadedPlayerList[0]; ;
            RoundWinner(winner);
        }
    }
}