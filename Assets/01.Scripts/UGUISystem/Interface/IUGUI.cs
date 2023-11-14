using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUGUI
{
    public void GenerateUI(Transform parent, EGenerateOption options);
    public void RemoveUI();
    public void UpdateUI();
}
