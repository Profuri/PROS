using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UIElements;
using PlayerManager = MonoPlayer.PlayerManager;

public class ScoreboardUI : MonoBehaviourPunCallbacks
{
    private UIDocument _document;

    [SerializeField] private VisualTreeAsset _userScoreboardAsset;

    private VisualElement _scoreboardContainer;

    private readonly List<ScoreboardEntry> _entries = new List<ScoreboardEntry>();

    private void Awake()
    {
        _document = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        var root = _document.rootVisualElement;
        FindElement(root);
    }

    private void FindElement(VisualElement root)
    {
        _scoreboardContainer = root.Q("scoreboard-container");
    }

    public void CreateNewEntry(Player newPlayer)
    {
        var r = (float)newPlayer.CustomProperties["R"];
        var g = (float)newPlayer.CustomProperties["G"];
        var b = (float)newPlayer.CustomProperties["B"];
        var color = new Color(r, g, b, 1);
        var entry = new ScoreboardEntry(color, newPlayer, _userScoreboardAsset, _scoreboardContainer);
        _entries.Add(entry);
    }

    public void UpdateScoreboard(Player targetPlayer)
    {
        var targetEntry = _entries.Find(x => x.Player.Equals(targetPlayer));
        targetEntry.UpdateEntry();
    }

    public void RemoveEntry(Player targetPlayer)
    {
        var targetEntry = _entries.Find(x => x.Player.Equals(targetPlayer));

        if (targetEntry is null)
        {
            return;
        }
        
        targetEntry.Remove();
        _entries.Remove(targetEntry);
    }
}
