using Photon.Realtime;

public interface IStageSystem
{
    public void Init();
    public void StageLeave();
    public void RemoveCurStage();
    public void GenerateNewStage();
    public void StageUpdate();
    public void OnDecideWinner(Player winner);
    public void Scoring(Player targetPlayer);
    public bool RoundCheck(out Player roundWinner);
}