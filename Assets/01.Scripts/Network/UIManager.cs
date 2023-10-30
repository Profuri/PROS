using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UIManager>();
            }
            return _instance;
        }
    }

    public void Init()
    {
        
    }
    public void CreateRoom()
    {
        NetworkManager.Instance.CreateRoom();
    }
    public void LeftRoom()
    {
        NetworkManager.Instance.LeaveRoom();
    }
}
