using System.Collections.Generic;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UIElements;

public class ScoreboardEntry
{
    private Player _player;
    public Player Player => _player;

    private VisualElement _parentRoot;
    private VisualElement _root;

    private List<VisualElement> _scoreElems = new List<VisualElement>();
    
    public ScoreboardEntry(Player player, VisualTreeAsset treeAsset, VisualElement parentRoot)
    {
        _player = player;
        _parentRoot = parentRoot;
        InstantiateTreeAsset(treeAsset);
    }
    
    private void InstantiateTreeAsset(VisualTreeAsset treeAsset)
    {
        _root = treeAsset.Instantiate().Q("user-scoreboard");
        _root.AddToClassList(_player.IsLocal ? "local" : "other");
        _scoreElems = _root.Query("score").ToList();
        _parentRoot.Add(_root);
        UpdateEntry();
    }

    public void UpdateEntry()
    {
        var score = _player.GetScore();

        for (var i = 0; i < 4; i++)
        {
            if (score >= i + 1)
            {
                _scoreElems[i].RemoveFromClassList("off");
            }
            else
            {
                _scoreElems[i].AddToClassList("off");
            }
        }
    }

    public void Remove()
    {
        _parentRoot.Remove(_root);
    }
}