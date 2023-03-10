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
    public float fireRate = 0;
    private float nextShot;

    Vector2 moveDirection;
    Vector2 mousePosition;

    // Health
    public int maxHealth = 100;
    public int initialHealth;
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>(100);

    public GameObject healthBarCanvas;
    HealthBar healthBar;

    public GameObject footprint;
    float footTime;

    Animator m_Animator;

    public FieldOfView fieldOfView;
 

    void Start()
    {
        healthBar = healthBarCanvas.transform.GetChild(0).GetComponent<HealthBar>();

        // Health
        currentHealth.Value = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        //Cursor.visible = false;
        nextShot = Time.time;

        m_Animator = gameObject.GetComponent<Animator>();

        fieldOfView = GameObject.Find("FieldOfView").GetComponent<FieldOfView>();

        if(IsOwner){
            gameObject.layer = LayerMask.NameToLayer("Top");
            healthBarCanvas.layer = LayerMask.NameToLayer("Top");
        }
    }


    // Update is called once per frame
    void Update()
    {
        footTime += Time.deltaTime;
        if (footTime > 0.5)
        {
            Instantiate(footprint, transform.position, transform.rotation);
            footTime = 0;
        }

       healthBar.SetHealth(currentHealth.Value);
        if (!IsOwner) return;
        
        // Movement
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // Weapon
        if(Input.GetKey(KeyCode.Space) && Time.time>nextShot)
        {
            m_Animator.SetTrigger("shootTrig");
            weapon.Fire(true);
            nextShot = Time.time + fireRate;
        }

        //Debug.Log(OwnerClientId + ", Health : " + currentHealth);

        // Movement
        moveDirection = new Vector2(moveX, moveY).normalized;
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        //var mousePos = Input.mousePosition;
        //mousePos.z = 10; // select distance = 10 units from the camera
        //mousePosition = GetComponent<Camera>().ScreenToWorldPoint(mousePos);
        if(IsOwner){
            fieldOfView.SetAimDirection(rb.rotation + 30);
            fieldOfView.SetOrigin(weapon.transform.position);
        }
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

    public void TakeDamage(int damage, bool fromPlayer, ulong shooterCliendId)
    {
        currentHealth.Value -= damage;
        if (currentHealth.Value <= 0)
        {
            ScoreManager.Instance.AddPoint(shooterCliendId);
            currentHealth.Value = maxHealth;
            transform.position = new Vector2(0f, 0f);
        }
    }

    private void LateUpdate() {
        rectTransform.rotation = Quaternion.Euler(0, 0,0);
        if (IsOwner){
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);        
        }
    }

    public void addHealth(int health)
    {
        currentHealth.Value += health;
        if(currentHealth.Value > maxHealth)
        {
            currentHealth.Value = maxHealth;
        }
    }
}