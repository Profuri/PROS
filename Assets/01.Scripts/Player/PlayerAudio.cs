using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioClip _onHitClip;

    public void PlayHitClip()
    {
        AudioManager.Instance.Play(_onHitClip);
    }
}
