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

        Debug.LogError($"OccupationStageSystem");
    }

    private void WinPlayer(Player player)
    {
        _winPlayer = player;
        _roundEnd = true;
    }
    
    public override void RoundCheck(Player player)
    {
        if (_roundEnd)
        {
            var winnerPlayer = _winPlayer;
            RoundWinner(winnerPlayer);            
        }
    }  
    public override void StageLeave()
    {
        base.StageLeave();
    }

    public override void GenerateNewStage(int index)
    {
        base.GenerateNewStage(index);
        
        _winPlayer = null;
        _roundEnd = false;
        
        if (NetworkManager.Instance.IsMasterClient == false) return;
        NetworkManager.Instance.PhotonView.RPC("SetOccupationSystemRPC",RpcTarget.All);
    }
    
    [PunRPC]
    private void SetOccupationSystemRPC()
    {
        OccupationStruct data = new OccupationStruct(_targetOccupationTime,
            minChangeTime: 20f,maxChangeTime: 40f,10f,_targetLayerMask);

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
        //It will be changed by fixed value
        //Vector3 randomPos = new Vector3(UnityEngine.Random.Range(-5,5f),0,0);
        
        //Test Code
        Vector3 randomPos = StageManager.Instance.CurStage.GetRandomSpawnPoint();
        
        _occupationSystem.SetOccupationPos(randomPos);
    }
}
