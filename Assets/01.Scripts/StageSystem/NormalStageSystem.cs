using MonoPlayer;
using Photon.Realtime;

public class NormalStageSystem : BaseStageSystem
{
    public override void Init(int mapIndex)
    {
        base.Init(mapIndex);

        PlayerManager.Instance.OnPlayerDead += (player) => 
        {
            var roundWinner = PlayerManager.Instance.LoadedPlayerList[0];
                if (roundWinner == null)
                {
                    return;
                }
                
                _runningStage = false;
                ++_round;
            
                ScoreManager.Instance.AddScore(roundWinner);
                StageManager.Instance.RoundWinner(roundWinner);
        };

    }
    public override void StageUpdate()
    {
        
    }

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
}