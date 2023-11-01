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

    /// <summary>
    /// OTC Dropout Execution Function
    /// </summary>
    /// <param name="attackDir"> Attack Object Moving Direction </param>
    /// <param name="otcPrevPos"> Otc Object Previous Moving Direction </param>
    /// <param name="otcCurPos"> Otc Object Current Moving Direction </param>
    public void PlayOTC(GameObject otcObj, Vector3 otcPrevPos, Vector3 otcCurPos, Vector3 attackDir)
    {
        Vector3 otcMovingDir = CalcMovingDir(otcPrevPos, otcCurPos);

        Vector3 otcDir = CalcOTCDir(attackDir,  otcMovingDir);
        if (otcDir.y < 0)
            otcDir *= -1;

        if (otcObj.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.AddForce(otcDir * _otcPower, ForceMode2D.Impulse);
        }
        else
            Debug.LogError("Otc Object Rigidbody2D not Found");
    }

    private Vector3 CalcOTCDir(Vector3 attackDir, Vector3 otcDir)
    {
        return (attackDir + otcDir).normalized;
    }

    private Vector3 CalcMovingDir(Vector3 prevPos, Vector3 curPos)
    {
        return (curPos - prevPos).normalized;
    }
}
