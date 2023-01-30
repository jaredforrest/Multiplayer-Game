using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BotController : NetworkBehaviour, IDamageable
{
    public SpriteRenderer spriteRenderer;
    public Sprite pistolSprite;
    public Sprite shotgunSprite;
    public Sprite rifleSprite;
    
    
    public Rigidbody2D rb;
    public Weapon weapon;

    // Health
    public int maxHealth;
    public int initialHealth;
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>(100);

    public GameObject healthBarCanvas;
    HealthBar healthBar;
    
    private float moveSpeed;
    public RectTransform rectTransform;
    private float weaponTimer = 0f;
    private float waitingTime;

    public int BotType;

    private void Start()
    {
        //Type of bot
        switch (BotType)
        {
            //Pistol
            case 1:
                maxHealth = 100;
                moveSpeed = 7;
                waitingTime = 1f;
                weapon.damage = 7;
                weapon.fireForce = 20f;
                spriteRenderer.sprite = pistolSprite;
                break;
            //Shotgun
            case 2:
                maxHealth = 100;
                moveSpeed = 5;
                waitingTime = 2f;
                weapon.damage = 10;
                weapon.fireForce = 20f;
                spriteRenderer.sprite = shotgunSprite;
                break;
            //Rifle
            case 3:
                maxHealth = 100;
                moveSpeed = 5;
                waitingTime = 1f;
                weapon.damage = 10;
                weapon.fireForce = 20f;
                spriteRenderer.sprite = rifleSprite;
                break;
            //Sniper
            case 4:
                maxHealth = 100;
                moveSpeed = 4;
                waitingTime = 4f;
                weapon.damage = 20;
                weapon.fireForce = 20f;
                spriteRenderer.sprite = pistolSprite;
                break;
            //Tank
            case 5:
                maxHealth = 300;
                moveSpeed = 2;
                waitingTime = 2f;
                weapon.damage = 10;
                weapon.fireForce = 20f;
                spriteRenderer.sprite = pistolSprite;
                break;
            //Machine Gun
            case 6:
                maxHealth = 100;
                moveSpeed = 5;
                waitingTime = 0.2f;
                weapon.damage = 2;
                weapon.fireForce = 20f;
                spriteRenderer.sprite = pistolSprite;
                break;
        }
        
        healthBar = healthBarCanvas.transform.GetChild(0).GetComponent<HealthBar>();

        // Health
        currentHealth.Value = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        healthBar.SetHealth(currentHealth.Value);
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        GameObject closest = Players[0];
        float player_distance = Mathf.Infinity;
        if (Players != null)
        {
            foreach (GameObject player in Players)
            {
                float distance = Vector2.Distance(transform.position, player.transform.position);
                if (distance < player_distance)
                {
                    closest = player;
                    player_distance = distance;
                }
            } 
            Vector2 aimDirection = new Vector2(closest.transform.position.x - weapon.transform.position.x, closest.transform.position.y - weapon.transform.position.y);
            float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            rb.rotation = aimAngle;
            
            rectTransform.rotation = Quaternion.Euler(0, 0,0);
            
            // Right is forward
            if (Vector3.Distance(transform.position, closest.transform.position) > 5f)
            {
                rb.AddForce(transform.right * moveSpeed);
            }
            else if (Vector3.Distance(transform.position, closest.transform.position) < 3f)
            {
                rb.AddForce(-transform.right * moveSpeed);
            }
            // Weapon
            if (Vector3.Distance(transform.position, closest.transform.position) < 10f)
            {
                weaponTimer += Time.deltaTime;
                if(weaponTimer > waitingTime){ 
                    weapon.FireServerRpc();
                    weaponTimer = 0;
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth.Value -= damage;
        
        if (currentHealth.Value <= 0)
        {
            Destroy(gameObject);
        }
    }
}
