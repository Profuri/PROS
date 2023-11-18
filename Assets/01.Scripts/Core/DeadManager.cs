using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using MonoPlayer;

public class DeadManager : MonoBehaviour
{
    private static DeadManager _instance;
    public static DeadManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<DeadManager>();
            }
            return _instance;
        }
    }
    [SerializeField] private float _deathDistance = 30f;
    private List<PlayerBrain> _playerBrainList;

    private Coroutine _coroutine;
    public void Init()
    {
        _playerBrainList = new List<PlayerBrain>();
    }

    public bool IsDeadPosition(Vector3 pos)
    {
        return Vector3.Distance(transform.position, pos) >= _deathDistance;
    }
}
