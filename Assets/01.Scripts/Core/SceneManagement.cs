using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    private static SceneManagement _instance;

    public static SceneManagement Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SceneManagement>();
            }

            return _instance;
        }
    }

    public event Action OnGameSceneLoaded;

    public void Init(Transform agent)
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        if (scene.name == ESceneName.Game.ToString())
        {
            OnGameSceneLoaded?.Invoke();
            //PhotonNetwork.Instantiate();
        }
    }
    
}
