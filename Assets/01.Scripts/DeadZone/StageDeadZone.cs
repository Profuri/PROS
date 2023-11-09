using System;
using UnityEngine;

public class StageDeadZone : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerBrain>(out var brain))
        {
            brain.OnPlayerDead();
        }
    }
}