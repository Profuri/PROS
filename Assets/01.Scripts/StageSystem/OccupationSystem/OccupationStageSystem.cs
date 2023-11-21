using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
using MonoPlayer;

public class OccupationStageSystem : BaseStageSystem
{
    private OccupationSystem _occupationSystem;
    [SerializeField] private LayerMask _targetLayerMask;
    [SerializeField] private float _targetOccupationTime;

    public Action OnTargetChangeTime;
    public Action<Player> OnPlayerWinEvent;

    public override void Init(int mapIndex)
    {
        base.Init(mapIndex);
        OnPlayerWinEvent += RoundCheck;
    }

    public override void StageLeave()
    {
        base.StageLeave();
        OnPlayerWinEvent -= RoundCheck;
        OnTargetChangeTime -= SetRandomOccupationPos;
    }

    public override void RoundCheck(Player player)
    {
        RoundWinner(player);            
    }  

    public override void GenerateNewStage(int index)
    {
        base.GenerateNewStage(index);
        
        if (false == NetworkManager.Instance.IsMasterClient) return;
        NetworkManager.Instance.PhotonView.RPC(nameof(SetOccupationSystemRPC),RpcTarget.All);
    }
    
    [PunRPC]
    private void SetOccupationSystemRPC()
    {
        OccupationStruct data = new OccupationStruct(_targetOccupationTime,
            minChangeTime: 60f,maxChangeTime: 80f, 10f, _targetLayerMask);

        if (_occupationSystem == null)
        {
            _occupationSystem = new OccupationSystem(this,data);
        }
        _occupationSystem.Init();
        SetRandomOccupationPos();
        
        OnTargetChangeTime += SetRandomOccupationPos;
    }
    private void SetRandomOccupationPos()
    {
        Vector3 randomPos = StageManager.Instance.CurStage.GetRandomSpawnPoint();
        
        _occupationSystem.SetOccupationPos(randomPos);
    }
}
