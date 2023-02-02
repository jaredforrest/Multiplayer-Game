using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot1 : MonoBehaviour, IDamageable
{
    // Movement
    public Rigidbody2D rb;
    public Weapon weapon;
    public Transform weaponTransform;

    public GameObject target;

    public float moveSpeed;

    float timer = 0f;
    float waitingTime = 2f;

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
        // Right is forward
        if (Vector3.Distance(transform.position, target.transform.position) > 5f)
        {
            rb.AddForce(transform.right * moveSpeed);
        }
        else if (Vector3.Distance(transform.position, target.transform.position) < 3f)
        {
            rb.AddForce(-transform.right * moveSpeed);
        }

        Vector2 aimDirection = new Vector2(target.transform.position.x - weaponTransform.position.x, target.transform.position.y - weaponTransform.position.y);
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        rb.rotation = aimAngle;

        if (Vector3.Distance(transform.position, target.transform.position) < 10f)
        {
            weapon.Fire();
        }
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

