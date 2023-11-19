using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Item")]
public class ItemSO : ScriptableObject
{
    [Tooltip("������ ȿ�� ���� �ð�")]
    public float ItemTime = 5f;
    [Tooltip("Ÿ�� ī��Ʈ")]
    public int UsableHitCnt = 3;
    public Sprite Sprite;
    [TextArea]
    public string Description;
}
