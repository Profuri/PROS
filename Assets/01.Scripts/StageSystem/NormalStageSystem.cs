using MonoPlayer;
using Photon.Realtime;
using UnityEngine;

public class NormalStageSystem : BaseStageSystem
{
    public override void Init(int mapIndex)
    {
        base.Init(mapIndex);
        PlayerManager.Instance.OnPlayerDead += RoundCheck;
    }

    public override void StageLeave()
    {
        base.StageLeave();
        PlayerManager.Instance.OnPlayerDead -= RoundCheck;
    }

    public override void RoundCheck(Player player)
    {
        if (PlayerManager.Instance.LoadedPlayerList.Count == 1)
        {
            var winner = PlayerManager.Instance.LoadedPlayerList[0];
            RoundWinner(winner);
        }
    }

}