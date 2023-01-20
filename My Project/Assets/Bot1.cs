using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot1 : MonoBehaviour
{
    // Movement
    public Rigidbody2D rb;
    public Weapon weapon;

    public GameObject player;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Weapon
        if(Input.GetKeyDown(KeyCode.Space))
        {
            weapon.Fire();
        }
    }

    private void FixedUpdate()
    {
        // Weapon 
        Vector2 aimDirection = new Vector2(player.transform.position.x - rb.position.x, player.transform.position.y - rb.position.y);
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = aimAngle;
    }
}

