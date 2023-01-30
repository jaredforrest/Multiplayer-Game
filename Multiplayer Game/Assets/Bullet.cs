using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Bullet : NetworkBehaviour
{
    public int damage;
    void Start()
    {
        // Destroy bullet after 1 second

       
        Destroy(gameObject, 1);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        if(damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        
        Destroy(gameObject);
    }
}
