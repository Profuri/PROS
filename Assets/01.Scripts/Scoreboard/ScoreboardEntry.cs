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

    private Color _originColor;
    private Color _inactivatedColor;
    
    public ScoreboardEntry(Color color, Player player, VisualTreeAsset treeAsset, VisualElement parentRoot)
    {
        _originColor = color;
        _inactivatedColor = color - new Color(0.15f, 0.15f, 0.15f, 0.2f);
        
        _player = player;
        _parentRoot = parentRoot;
        InstantiateTreeAsset(treeAsset);
    }
    
    private void InstantiateTreeAsset(VisualTreeAsset treeAsset)
    {
        _root = treeAsset.Instantiate().Q("user-scoreboard");
        _scoreElems = _root.Query("score").ToList();
        _parentRoot.Add(_root);
        UpdateEntry();
    }

    public void UpdateEntry()
    {
        var score = (int)_player.CustomProperties["Score"];

        for (var i = 0; i < 4; i++)
        {
            if (score >= i + 1)
            {
                _scoreElems[i].style.unityBackgroundImageTintColor = new StyleColor(_originColor);
            }
            else
            {
                _scoreElems[i].style.unityBackgroundImageTintColor = new StyleColor(_inactivatedColor);
            }
        }
    }

    public void Remove()
    {
        _parentRoot.Remove(_root);
    }
}