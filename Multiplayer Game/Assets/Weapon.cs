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

    public void Fire(bool fromPlayer)
    {
        FireServerRpc(fromPlayer);
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void FireServerRpc(bool fromPlayer, ServerRpcParams serverRpcParams = default)
    {
        if (!_isFlashing)
        {
            StartCoroutine(muzzleFlash());
        }
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet _bullet = bullet.GetComponent<Bullet>();
        _bullet.damage = damage;
        _bullet.fromPlayer = fromPlayer;
        _bullet.shooterCliendId = serverRpcParams.Receive.SenderClientId;
        bullet.GetComponent<Rigidbody2D>().AddForce(firePoint.up * fireForce, ForceMode2D.Impulse);
        bullet.GetComponent<NetworkObject>().Spawn();
    }

    IEnumerator muzzleFlash()
    {
        _isFlashing = true;
        muzzleFlashActiveClientRpc(true);
        var framesFlashed = 0;
        while (framesFlashed <= FramesToFlash)
        {
            framesFlashed++;
            yield return null;
        }
        _isFlashing = false;
        muzzleFlashActiveClientRpc(false);
    }

    [ClientRpc]
    void muzzleFlashActiveClientRpc(bool state)
    {
        muzzleFlashSprite.SetActive(state);

    }
}
