using Photon.Realtime;
using UnityEngine;

public class StageWinnerScreen : UGUIComponent
{
    [SerializeField] private float _activeTime;
    private float _currentTime;

    [SerializeField] private PlayerCard _winnerCard;

    public override void GenerateUI(Transform parent, EGenerateOption options)
    {
        base.GenerateUI(parent, options);
        
        _winnerCard.GenerateUI(_winnerCard.transform.parent, EGenerateOption.NONE);
        
        _currentTime = 0f;
    }

    public override void UpdateUI()
    {
        _currentTime += Time.deltaTime;
        var percent = _currentTime / _activeTime;
        
        if (percent >= 1f)
        {
            Application.Quit();
        }
    }
    
    public void SetWinner(Player player)
    {
        var r = (float)player.CustomProperties["R"];
        var g = (float)player.CustomProperties["G"];
        var b = (float)player.CustomProperties["B"];
        var color = new Color(r, g, b, 1);

        _winnerCard.SetColor(color);
        _winnerCard.SetNickName(player.NickName);
        _winnerCard.Winning();
    }
}