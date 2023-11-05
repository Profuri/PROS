using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SmokeParticleTesting : MonoBehaviour
{
    [SerializeField] private ParticleSystem _smokeParticle;
    [SerializeField] private ParticleSystem _explosionParticle;

    [SerializeField] private float _force;

    private Rigidbody2D _rigidbody;
    private Vector2 _mousePosition;

    private Camera _mainCam;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _mainCam = Camera.main;
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            _mousePosition = Input.mousePosition;
            _mousePosition = _mainCam.ScreenToWorldPoint(_mousePosition);

            var dir = (_mousePosition - (Vector2)transform.position).normalized;
            var angle = Mathf.Atan2(-dir.y, -dir.x) * Mathf.Rad2Deg;

            _explosionParticle.transform.position = transform.position;
            _explosionParticle.Play();
            
            _smokeParticle.transform.rotation = Quaternion.Euler(0, 0, angle - 60);
            _smokeParticle.Play();

            _rigidbody.AddForce(dir * _force, ForceMode2D.Impulse);
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            _smokeParticle.Stop();
            _rigidbody.velocity = Vector2.zero;
            transform.position = Vector2.zero;
        }
    }
}
