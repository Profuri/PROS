using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizingCircleCollider : MonoBehaviour
{
    [SerializeField] Transform LeftLegEndPos;
    [SerializeField] Transform RightLegEndPos;
    [SerializeField] BoxCollider2D _checkgroundCollider;

    public void FixedUpdate()
    {
        float leftY = LeftLegEndPos.position.y;
        float rightY = RightLegEndPos.position.y;
        bool left = Mathf.Min(leftY, rightY) == leftY ? true : false;

        if (left) _checkgroundCollider.transform.position = LeftLegEndPos.position;
        else _checkgroundCollider.transform.position = RightLegEndPos.position;

        //float radius = Mathf.Abs(Mathf.Max(Vector3.Distance(ColliderCenterPos.position, LeftLegEndPos.position)
        //    , Vector3.Distance(ColliderCenterPos.position, RightLegEndPos.position)));

        //Mathf.Clamp(radius, 0.0f, 1.0f);

        //_collider.radius = radius * 2;
    }
}
