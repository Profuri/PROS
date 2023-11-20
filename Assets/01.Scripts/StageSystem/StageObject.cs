using System.Collections.Generic;
using UnityEngine;

public class StageObject : PoolableMono
{
    private List<Vector3> _spawnPoints = new List<Vector3>();
    public List<Vector3> SpawnPoints => _spawnPoints;

    private List<BasePlatform> _platforms = new List<BasePlatform>();
    private List<BaseWall> _walls = new List<BaseWall>();

    public List<BasePlatform> Platforms => _platforms;
    public List<BaseWall> Walls => _walls;

    public void Setting(Vector3 position = default, Quaternion rotation = default)
    {
        Reset();
        _platforms.Clear();
        _walls.Clear();
        _spawnPoints.Clear();
        
        transform.position = position;
        transform.rotation = rotation;

        // GetComponentsInChildren(_platforms);
        // GetComponentsInChildren(_walls);

        // for (var i = 0; i < _platforms.Count; i++)
            // _platforms[i].Init(i);
        // for(var i = 0; i < _walls.Count; i++)
            // _walls[i].Init();

        var pointsTrm = transform.Find("Points");
        for (var i = 0; i < pointsTrm.childCount; i++)
        {
            var point = pointsTrm.GetChild(i);
            _spawnPoints.Add(point.position);
        }
    }

    private void Reset()
    {
        foreach (var platform in _platforms)
        {
            platform.Reset();
        }

        foreach (var wall in _walls)
        {
            wall.Reset();
        }
    }

    public override void Init()
    {
        
    }
}