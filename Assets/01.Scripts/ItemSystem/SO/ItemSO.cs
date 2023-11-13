using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Item")]
public class ItemSO : ScriptableObject
{
    [Tooltip("아이템 효과 지속 시간")]
    public float ItemTime = 5f;
    [Tooltip("타격 카운트")]
    public int UsableHitCnt = 3;
    public Sprite Sprite;
    [TextArea]
    public string Description;
}
