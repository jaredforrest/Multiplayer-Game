using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{
    // Movement
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Weapon weapon;

    Vector2 moveDirection;
    Vector2 mousePosition;

    // Health
    public int maxHealth = 100;
    public int currentHealth;

    //public GameObject healthBarCanvas;
    public GameObject healthBarCanvasPrefab;
    HealthBar healthBar;

    void Start()
    {
        GameObject healthBarCanvas = Instantiate(healthBarCanvasPrefab);

        StayAboveTarget sat = healthBarCanvas.GetComponent<StayAboveTarget>();
        sat.targetToFollow = transform;

        healthBar = healthBarCanvas.transform.GetChild(0).GetComponent<HealthBar>();

        // Health
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        // Movement
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // Weapon
        if(Input.GetKeyDown(KeyCode.Space))
        {
            weapon.Fire();
        }

        // Movement
        moveDirection = new Vector2(moveX, moveY).normalized;
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void FixedUpdate()
    {
        // Movement
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);

        // Weapon
        Vector2 aimDirection = mousePosition - rb.position;
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = aimAngle;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        healthBar.SetHealth(currentHealth);
    }
}
