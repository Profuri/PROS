using Photon.Pun;
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

    public override void Init(int index)
    {
        base.Init(index);

        _velocity = _speed * Time.deltaTime;
        _startPos = transform.position;
        
        if (NetworkManager.Instance.IsMasterClient)
        {
            StartCoroutine(UpdatePositionRoutine());
        }
    }

    public override void Reset()
    {
        base.Reset();
        StopAllCoroutines();
    }

    private IEnumerator UpdatePositionRoutine()
    {
        while (true)
        {
            if (Vector2.Distance(_startPos, transform.position) > _distance * 0.5f)
            {
                _velocity *= -1f;
            }

            var nextPos = Vector2.zero;
            
            if (_moveAxis == MoveAxis.X)
            {
                nextPos = transform.position + new Vector3(_velocity, 0);
            }
            else
            {
                nextPos = transform.position + new Vector3(0, _velocity);
            }
            
            StageManager.Instance.SetPosition(Index, nextPos);

            yield return null;
        }
    }
}
