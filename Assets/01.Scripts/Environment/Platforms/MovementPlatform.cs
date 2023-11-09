using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveAxis
{
    X, Y
}

public class MovementPlatform : BasePlatform
{
    [SerializeField] private float _distance;
    [SerializeField] private float _speed;
    [SerializeField] private MoveAxis _moveAxis;

    private Vector2 _startPos;
    private float _velocity;

    private void Start()
    {
        _velocity = _speed * Time.deltaTime;
        _startPos = transform.position;
    }

    private void Update()
    {
        if(Vector2.Distance(_startPos, transform.position) > _distance * 0.5f)
        {
            _velocity *= -1f;
        }
        if (_moveAxis == MoveAxis.X)
            transform.position += new Vector3(_velocity, 0);
        else transform.position += new Vector3(0, _velocity);
    }
}
