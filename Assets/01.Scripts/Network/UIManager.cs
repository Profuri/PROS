using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
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

    [SerializeField] private TMP_Text _text;
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
