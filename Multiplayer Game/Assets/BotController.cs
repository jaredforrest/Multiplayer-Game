using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

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
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>(100);
    public GameObject healthBarCanvas;
    HealthBar healthBar;
    
    public RectTransform rectTransform;
    private float weaponTimer = 0f;
    private float waitingTime;

    public int BotType;

    NavMeshAgent agent;

    private void Start()
    {
        setBotTypeClientRpc(BotType);
        
        healthBar = healthBarCanvas.transform.GetChild(0).GetComponent<HealthBar>();

        // Health
        if(IsOwner)
        {
            currentHealth.Value = maxHealth;
        }
        healthBar.SetMaxHealth(maxHealth);
    }
    
    [ClientRpc]
     private void setBotTypeClientRpc(int BotType)
     {
        //Type of bot
        switch (BotType)
        {
            //Pistol
            case 1:
                maxHealth = 100;
                agent.speed = 7;
                waitingTime = 1f;
                weapon.damage = 7;
                weapon.fireForce = 20f;
                spriteRenderer.sprite = pistolSprite;
                break;
            //Shotgun
            case 2:
                maxHealth = 100;
                agent.speed = 5;
                waitingTime = 2f;
                weapon.damage = 10;
                weapon.fireForce = 20f;
                spriteRenderer.sprite = shotgunSprite;
                break;
            //Rifle
            case 3:
                maxHealth = 100;
                agent.speed = 5;
                waitingTime = 1f;
                weapon.damage = 10;
                weapon.fireForce = 20f;
                spriteRenderer.sprite = rifleSprite;
                break;
            //Sniper
            case 4:
                maxHealth = 100;
                agent.speed = 4;
                waitingTime = 4f;
                weapon.damage = 20;
                weapon.fireForce = 20f;
                spriteRenderer.sprite = pistolSprite;
                break;
            //Tank
            case 5:
                maxHealth = 300;
                agent.speed = 2;
                waitingTime = 2f;
                weapon.damage = 10;
                weapon.fireForce = 20f;
                spriteRenderer.sprite = pistolSprite;
                break;
            //Machine Gun
            case 6:
                maxHealth = 100;
                agent.speed = 5;
                waitingTime = 0.2f;
                weapon.damage = 2;
                weapon.fireForce = 20f;
                spriteRenderer.sprite = pistolSprite;
                break;
        }

     }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        healthBar.SetHealth(currentHealth.Value);

        // Only run the rest of the code on ther server
        if (!IsOwner || !IsSpawned)
        {
            return;
        }

        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        float player_distance = Mathf.Infinity;
        Vector2 closestPosition = new Vector2(0, 0);
        foreach (GameObject player in Players)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance < player_distance)
            {
                closestPosition = player.transform.position;
                player_distance = distance;
            }
        }

        agent.SetDestination(new Vector3(closestPosition.x, closestPosition.y, transform.position.z));
        
        Vector2 aimDirection = new Vector2(closestPosition.x - weapon.transform.position.x, closestPosition.y - weapon.transform.position.y);
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        rb.rotation = aimAngle;
        
        // Weapon
        if (Vector3.Distance(transform.position, closestPosition) < 10f)
        {
            weaponTimer += Time.deltaTime;
                if(weaponTimer > waitingTime){ 
                weapon.Fire(false);
                weaponTimer = 0;
            }
        }
    }

    public void TakeDamage(int damage, bool fromPlayer, ulong shooterCliendId)
    {
        if(!fromPlayer)
        {
            return;
        }

        currentHealth.Value -= damage;
        
        if (currentHealth.Value <= 0)
        {
            ScoreManager.Instance.AddPoint(shooterCliendId);
            Destroy(gameObject);
        }
    }
    public void LateUpdate()
    {
            rectTransform.rotation = Quaternion.Euler(0, 0,0);
    }
}
