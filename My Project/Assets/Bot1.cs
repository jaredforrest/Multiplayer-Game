using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot1 : MonoBehaviour, IDamageable
{
    // Movement
    public Rigidbody2D rb;
    public Weapon weapon;

    public GameObject target;
    public float speed = 3f;

    float timer = 0f;
    float waitingTime = 5f;

    // Health
    public int maxHealth = 50;
    public int currentHealth;

    public GameObject healthBarCanvas;
    private HealthBar healthBar;

    void Start()
    {
        // Set health to max
        healthBar = healthBarCanvas.transform.GetChild(0).GetComponent<HealthBar>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

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
        Vector2 aimDirection = new Vector2(target.transform.position.x - rb.position.x, target.transform.position.y - rb.position.y);
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = aimAngle;

        if (Vector3.Distance(transform.position, target.transform.position) > 2f){
             transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
        }
        // Weapon 
    }
    private void OnDestroy()
    {
        Destroy(healthBarCanvas);
    }

    public void TakeDamage(int damage)
    {
        if (damage > currentHealth)
        {
            Destroy(gameObject);
        } 
        else
        {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        }

    }
}

