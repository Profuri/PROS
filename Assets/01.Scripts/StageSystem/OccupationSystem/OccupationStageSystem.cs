using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
public class OccupationStageSystem : BaseStageSystem
{
    private OccupationSystem _occupationSystem;
    [SerializeField] private LayerMask _targetLayerMask;
    [SerializeField] private float _targetOccupationTime;
    public Action OnTargetChangeTime;
    public Action<Player> OnPlayerWinEvent;

    private Player _winPlayer;
    private bool _roundEnd = false;
    

    public override void Init(int mapIndex)
    {
        base.Init(mapIndex);
        _roundEnd = false;
        OnPlayerWinEvent += WinPlayer;
    }

    private void WinPlayer(Player player)
    {
        _winPlayer = player;
        _roundEnd = true;
    }
    public override bool RoundCheck(out Player winnerPlayer)
    {
        winnerPlayer = null;
        if (_roundEnd)
        {
            winnerPlayer = _winPlayer;
        }
        return _roundEnd;
    }

    public override void GenerateNewStage(int index)
    {
        base.GenerateNewStage(index);
        
        if (NetworkManager.Instance.IsMasterClient == false) return;
        NetworkManager.Instance.PhotonView.RPC("SetOccupationSystemRPC",RpcTarget.All);
    }

    private void SetOccupationSystemRPC()
    {
        OccupationStruct data = new OccupationStruct(_targetOccupationTime,
            minChangeTime: 20f,maxChangeTime: 40f,5f,_targetLayerMask);
        
        _occupationSystem = new OccupationSystem(this,data);
        SetRandomOccupationPos();
        
        OnTargetChangeTime += SetRandomOccupationPos;
    }
    private void SetRandomOccupationPos()
    {
        //It will be changed by fixed value
        //Vector3 randomPos = new Vector3(UnityEngine.Random.Range(-5,5f),0,0);
        
        //Test Code
        Vector3 randomPos = Vector3.zero - new Vector3(0 ,-2 ,0);
        
        _occupationSystem.SetOccupationPos(randomPos);
    }
}
