using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
        //NetworkManager.Instance = GetComponent<NetworkManager>();
        NetworkManager.Instance.Init();
        //UIManager.Instance.Init();
        SceneManagement.Instance.Init(this.transform);
        SceneManagement.Instance.OnGameSceneLoaded += () => PhotonNetwork.Instantiate("Player",transform.position,Quaternion.identity);
    }
}
