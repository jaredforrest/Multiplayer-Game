using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour, IDamageable
{

    // Movement
    public float moveSpeed = 5f;
    public RectTransform rectTransform;
    
    public Rigidbody2D rb;
    public Weapon weapon;

    Vector2 moveDirection;
    Vector2 mousePosition;

    // Health
    public int maxHealth = 100;
    public int initialHealth;
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>(100);

    public GameObject healthBarCanvas;
    HealthBar healthBar;

    public GameObject cameraPrefab;
    public GameObject camera;
 

    void Start()
    {
        if(IsOwner){
            camera = Instantiate(cameraPrefab);
        }
        
        healthBar = healthBarCanvas.transform.GetChild(0).GetComponent<HealthBar>();

        // Health
        currentHealth.Value = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        //Cursor.visible = false;


    }

    // Update is called once per frame
    void Update()
    {
       healthBar.SetHealth(currentHealth.Value);
        if (!IsOwner) return;
        
        // Movement
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // Weapon
        if(Input.GetKeyDown(KeyCode.Space))
        {
            weapon.FireServerRpc();
        }

        //Debug.Log(OwnerClientId + ", Health : " + currentHealth);

        // Movement
        moveDirection = new Vector2(moveX, moveY).normalized;
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        //var mousePos = Input.mousePosition;
        //mousePos.z = 10; // select distance = 10 units from the camera
        //mousePosition = GetComponent<Camera>().ScreenToWorldPoint(mousePos);

    
    }

    private void FixedUpdate()
    {
        // Movement
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);

        // Weapon
        Vector2 aimDirection = mousePosition - rb.position;
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        rb.rotation = aimAngle;
    }

    public void TakeDamage(int damage)
    {
        currentHealth.Value -= damage;
    }

    private void LateUpdate() {
        rectTransform.rotation = Quaternion.Euler(0, 0,0);
        if (IsOwner){
        camera.transform.position = new Vector3(transform.position.x, transform.position.y, -10);        
        }
    }
}
