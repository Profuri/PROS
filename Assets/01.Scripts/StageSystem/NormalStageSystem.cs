using MonoPlayer;
using Photon.Realtime;

public class NormalStageSystem : BaseStageSystem
{
    public override bool RoundCheck(out Player roundWinner)
    {
        roundWinner = null;
        if (PlayerManager.Instance.LoadedPlayerList.Count == 1)
        {
            roundWinner = PlayerManager.Instance.LoadedPlayerList[0];
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void GenerateNewStage()
    {
        
    }

    public override void RemoveCurStage()
    {
        
    }
}