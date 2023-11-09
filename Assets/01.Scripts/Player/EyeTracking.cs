using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeTracking : MonoBehaviour
{
    [SerializeField] Transform _eyeCenter;

    private Vector2 _targetPos;
    private PlayerBrain _brain;

    private void Awake()
    {
        _brain = GetComponent<PlayerBrain>();    
    }

    private void Update()
    {
        _targetPos = _brain.InputSO.CurrentMousePos;

        Vector2 direction = new Vector2(
            _targetPos.x - transform.position.x,
            _targetPos.y - transform.position.y
        );

        transform.up = direction;
    }
}
