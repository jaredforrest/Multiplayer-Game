using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Bullet : NetworkBehaviour
{
    public int damage;
    public ulong shooterCliendId = 0;
    public int speed = 10;
    public bool fromPlayer;
    void Start()
    {
        if (!IsOwner || !IsSpawned)
        {
            return;
        }

        // Disabled because causes errors
        // Destroy bullet after 1 second
        //Destroy(gameObject, 1);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsOwner || !IsSpawned)
        {
            return;
        }

        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        if(damageable != null)
        {
            damageable.TakeDamage(damage,fromPlayer, shooterCliendId);
        }

        gameObject.GetComponent<NetworkObject>().Despawn();
    }

    void FixedUpdate() {
        GetComponent<Rigidbody2D>().velocity = new Vector3(1.0f,1.0f,1.0f);
    }
}
