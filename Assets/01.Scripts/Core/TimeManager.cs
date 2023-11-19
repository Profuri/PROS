using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
public class TimeManager : MonoBehaviour
{
    private static TimeManager _instance;
    public static TimeManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<TimeManager>();
            }
            return _instance;
        }
    }
    [SerializeField] private float _freezeTime = 0.3f;
    [SerializeField] private float _freezeTimeScale = 0.1f;

    private Coroutine _coroutine;
    public void Init(){}


    public void SetTimeScale()
    {
        if(NetworkManager.Instance.IsMasterClient)
        {
            NetworkManager.Instance.PhotonView.RPC(nameof(SetTimeScaleRPC),RpcTarget.All);    
        }
    }
    
    [PunRPC]
    private void SetTimeScaleRPC()
    {
        if(_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        _coroutine = StartCoroutine(SetTimeScaleCor(_freezeTime));
    }

    private IEnumerator SetTimeScaleCor(float freezeTime)
    {
        Time.timeScale = _freezeTimeScale;
        yield return new WaitForSeconds(freezeTime);
        Time.timeScale = 1;
    }   

}
