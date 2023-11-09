using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct PoolingPair{
    public PoolableMono prefab;
    public int count;
}

[CreateAssetMenu(menuName = "SO/PoolingList")]
public class PoolingListSO : ScriptableObject{
    public List<PoolingPair> pairs;
    
}