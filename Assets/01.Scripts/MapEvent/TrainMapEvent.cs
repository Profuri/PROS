using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TrainMapEvent : BaseMapEvent
{
    private Train _curTrain;
    public override void StartEvent()
    {
        Debug.Log("TrainMapEvent");
        ExecuteEvent();
    }

    public override void EndEvent()
    {
        if(NetworkManager.Instance.IsMasterClient)
        {
            if(_curTrain.gameObject != null)
            {
                _curTrain = null;
                PhotonNetwork.Destroy(_curTrain.gameObject);
            }
        }
    }

    public override void ExecuteEvent()
    {
        _curTrain = PhotonNetwork.Instantiate("Train",GetRandomPos(),Quaternion.identity).GetComponent<Train>();
        _curTrain.Init();
    }

    private Vector3 GetRandomPos()
    {
        float x = Random.Range(_mapBound.minX,_mapBound.maxX);
        float y = Random.Range(_mapBound.minY,_mapBound.maxY);

        return new Vector3(x,y,0);
    }
}
