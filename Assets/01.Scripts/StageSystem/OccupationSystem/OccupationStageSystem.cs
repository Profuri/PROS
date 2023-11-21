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
        OnTargetChangeTime += SetOccupationSystem;
    }

    public override void StageLeave()
    {
        base.StageLeave();
        OnPlayerWinEvent -= RoundCheck;
        OnTargetChangeTime -= SetOccupationSystem;
    }

    public override void RoundCheck(Player player)
    {
        NetworkManager.Instance.PhotonView.RPC(nameof(RoundWinnerRPCOccupation), RpcTarget.All,player);
    }

    [PunRPC]
    private void RoundWinnerRPCOccupation(Player player)
    {
        RoundWinner(player);
    }

    public override void GenerateNewStage(int index)
    {
        base.GenerateNewStage(index);
        
        if (NetworkManager.Instance.IsMasterClient)
        {
            SetOccupationSystem();
        }
    }
    
    private void SetOccupationSystem()
    {
        OccupationStruct data = new OccupationStruct(_targetOccupationTime,
            minChangeTime: 60f,maxChangeTime: 80f, 10f, _targetLayerMask);

        if (null == _occupationSystem)
        {
            _occupationSystem = new OccupationSystem(this,data);
        }
        _occupationSystem.Init();

        Vector3 randomPos = StageManager.Instance.CurStage.GetRandomSpawnPoint();

        _occupationSystem.SetOccupationPos(randomPos);
    }
}
