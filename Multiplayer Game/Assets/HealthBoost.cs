using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HealthBoost : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if (!IsOwner || !IsSpawned)
        {
            return;
        }

        if(other.CompareTag("Player")){
            AddHealth(other);

        }
    }

    private void AddHealth(Collider2D player) {
        player.gameObject.GetComponent<PlayerController>().addHealth(15);
        gameObject.GetComponent<NetworkObject>().Despawn();
    }
}
