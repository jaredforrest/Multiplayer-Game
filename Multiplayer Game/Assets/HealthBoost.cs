using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HealthBoost : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsOwner || !IsSpawned)
        {
            return;
        }

        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        if(player != null)
        {
            player.addHealth(15);
            gameObject.GetComponent<NetworkObject>().Despawn();
        }

    }
}
