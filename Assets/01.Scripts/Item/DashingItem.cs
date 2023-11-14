using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DashingItem : BaseItem
{
    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        _moveDir = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized;
        transform.position = new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
        var moveSpeed = Random.Range(1f, 2f);
        _movementSpeed = moveSpeed;
    }

    public override void OnTakeItem(Player takenPlayer)
    {
            
    }

    private void Update()
    {
        transform.Translate(_moveDir * (_movementSpeed * Time.deltaTime), Space.World);
    }

    public override void UpdateItem()
    {
        base.UpdateItem();
        ReflectMove();
    }

    private void ReflectMove()
    {
        
    }

    //ContactPoint2D[] contacts = new ContactPoint2D[2];
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    Debug.Log("GetContacts");
    //    collision.GetContacts(contacts);
    //    _moveDir = Vector2.Reflect(_moveDir, -1 * contacts[0].normal);
    //}
    private void OnCollisionEnter2D(Collision2D collision)
    {
        _moveDir = Vector2.Reflect(_moveDir, collision.contacts[0].normal);
        Debug.Log("contact");
    }
}
