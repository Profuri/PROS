using System.Collections.Generic;
using UnityEngine;

public class StageObject : MonoBehaviour
{
    private List<Vector3> _spawnPoints = new List<Vector3>();
    public List<Vector3> SpawnPoints => _spawnPoints;

    public void Setting(Vector3 position = default, Quaternion rotation = default)
    {
        _spawnPoints.Clear();
        
        transform.position = position;
        transform.rotation = rotation;

        var pointsTrm = transform.Find("Points");
        for (var i = 0; i < pointsTrm.childCount; i++)
        {
            var point = pointsTrm.GetChild(i);
            _spawnPoints.Add(point.position);
        }
    }
}