using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TrainMapEvent : BaseMapEvent
{
    public override void StartEvent()
    {
        Debug.Log("TrainMapEvent");
        ExecuteEvent();
    }

    public override void EndEvent()
    {
        
    }

    public override void ExecuteEvent()
    {
        Train train = PhotonNetwork.Instantiate("Train",GetRandomPos(),Quaternion.identity).GetComponent<Train>();
        train.Init();
    }

    private Vector3 GetRandomPos()
    {
        float x = Random.Range(_mapBound.minX,_mapBound.maxX);
        float y = Random.Range(_mapBound.minY,_mapBound.maxY);

        return new Vector3(x,y,0);
    }
}
