    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Weapon : NetworkBehaviour
{
    public int damage;
    public GameObject muzzleFlashSprite;
    public int FramesToFlash;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireForce;
    bool _isFlashing = false;

    private void Start()
    {
        muzzleFlashSprite.SetActive(false);
    }

    public void Fire()
    {
        FireServerRpc();
        if (!_isFlashing)
        {
            StartCoroutine(muzzleFlash());
        }
        
    }

    [ServerRpc]
    public void FireServerRpc()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Bullet>().damage = damage;
        bullet.GetComponent<Rigidbody2D>().AddForce(firePoint.up * fireForce, ForceMode2D.Impulse);
        bullet.GetComponent<NetworkObject>().Spawn();
    }

    IEnumerator muzzleFlash()
    {
        _isFlashing = true;
        muzzleFlashSprite.SetActive(true);
        var framesFlashed = 0;
        while (framesFlashed <= FramesToFlash)
        {
            framesFlashed++;
            yield return null;
        }
        muzzleFlashSprite.SetActive(false);
        _isFlashing = false;
    }
}
