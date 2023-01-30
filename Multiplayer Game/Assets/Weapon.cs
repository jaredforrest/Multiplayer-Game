using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Weapon : NetworkBehaviour
{
    public int damage;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireForce;

    [ServerRpc]
    public void FireServerRpc()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Bullet>().damage = damage;
        bullet.GetComponent<Rigidbody2D>().AddForce(firePoint.up * fireForce, ForceMode2D.Impulse);
        bullet.GetComponent<NetworkObject>().Spawn(true);
    }
}
