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

}
