using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OTCManager : MonoBehaviour
{
    private static OTCManager _instance;

    public static OTCManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<OTCManager>();
            }
            return _instance;
        }
    }

    [SerializeField] private float _otcPower = 10f;

    public GameObject attackObj;
    public GameObject otcObj;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayOTC(attackObj, otcObj);
        }
    }

    /// <summary>
    /// OTC Dropout Execution Function
    /// </summary>
    /// <param name="attackObj">Objects that don't fly away</param>
    /// <param name="otcObj">Objects to fly away</param>
    public void PlayOTC(GameObject attackObj, GameObject otcObj)
    {
        if (attackObj == null)
        {
            Debug.LogError("Attack Object is null");
            return;   
        }
        if (otcObj == null) 
        {
            Debug.LogError("OTC Object is null");
            return;
        }
        
        Vector2 attackObjDir = CalcMovingDir(attackObj);
        Vector2 otcObjDir = CalcMovingDir(otcObj);
        Debug.Log($"AtkObjDir : {attackObjDir}, OtcObjDir : {otcObjDir}");

        Vector2 otcDir = CalcOTCDir(attackObjDir, otcObjDir);
        if (otcDir.y < 0)
        {
            otcDir.y *= -1;
        }
        Debug.Log($"OtcDIr : {otcDir}");
    
        if(otcObj.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            Debug.Log($"Object Otc");
            rb.transform.Translate(otcDir * 10);
        }
    }

    private Vector2 CalcOTCDir(Vector2 attackDir, Vector2 otcDir)
    {
        return (attackDir + otcDir).normalized;
    }

    private Vector2 CalcMovingDir(GameObject obj)
    {
        Vector2 movingDir = new Vector2();
        if (obj == null)
            Debug.LogError("Moving Obj is null");
        
        if (obj.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            movingDir = rb.velocity.normalized;
        }
        else
        {
            Debug.LogError("Don't Find Rigidbody in OTC Object");
        }

        return movingDir;
    }
}
