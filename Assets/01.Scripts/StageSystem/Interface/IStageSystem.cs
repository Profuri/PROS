using Photon.Realtime;

public interface IStageSystem
{
    public void Init(int mapIndex);
    public void StageLeave();
    public void RemoveCurStage();
    public void GenerateNewStage(int mapIndex);
    public void StageUpdate();
    public void RoundCheck(Player player);
}