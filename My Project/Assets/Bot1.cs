using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot1 : MonoBehaviour
{
    // Movement
    public Rigidbody2D rb;
    public Weapon weapon;

    public GameObject player;
    public float speed = 3f;

    float timer = 0f;
    float waitingTime = 5f;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Weapon
        timer += Time.deltaTime;
        if(timer > waitingTime){
            weapon.Fire();
            timer = 0;
        }
    }

    private void FixedUpdate()
    {
        Vector2 aimDirection = new Vector2(player.transform.position.x - rb.position.x, player.transform.position.y - rb.position.y);
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = aimAngle;

        if (Vector3.Distance(transform.position, player.transform.position) > 2f){
             transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
        }
        // Weapon 
    }
}

