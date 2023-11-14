using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TrainMapEvent : BaseMapEvent
{
    public override void StartEvent()
    {
        ExecuteEvent();
    }

    public override void EndEvent()
    {
        
    }

    public override void ExecuteEvent()
    {
        Train train = PhotonNetwork.Instantiate("Train",Vector3.zero,Quaternion.identity).GetComponent<Train>();
        train.Init();
    }
}
