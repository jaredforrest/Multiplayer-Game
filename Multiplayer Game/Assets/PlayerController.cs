using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

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

    public GameObject cameraPrefab;
    public new GameObject camera;

    public GameObject footprint;
    float footTime;

    Animator m_Animator;

    // JoyStick
    public GameObject joystickCanvas;
    FixedJoystick joystick;
    
 

    void Start()
    {
        if(IsOwner){
            camera = Instantiate(cameraPrefab);
            GameObject _joystickCanvas = Instantiate(joystickCanvas);
            joystick = _joystickCanvas.transform.GetChild(0).GetComponent<FixedJoystick>();
            Button fireButton = _joystickCanvas.transform.GetChild(0).GetChild(1).GetComponent<Button>();
            fireButton.onClick.AddListener(delegate{Fire();});
        }
        
        healthBar = healthBarCanvas.transform.GetChild(0).GetComponent<HealthBar>();

        // Health
        currentHealth.Value = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        //Cursor.visible = false;
        nextShot = Time.time;

        m_Animator = gameObject.GetComponent<Animator>();
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
        // float moveX = Input.GetAxis("Horizontal");
        // float moveY = Input.GetAxis("Vertical");

        //JoyStick Movement
        float moveX = joystick.Horizontal;
        float moveY = joystick.Vertical;

        // Weapon
        if(Input.GetKey(KeyCode.Space) && Time.time>nextShot)
        {
            Fire();
        }

        // Movement
        moveDirection = new Vector2(moveX, moveY).normalized;
        //mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }


    private void FixedUpdate()
    {
        // Movement
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);

        // Weapon
        //Vector2 aimDirection = mousePosition - rb.position;
        if(moveDirection != Vector2.zero)
        {
            float aimAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            rb.rotation = aimAngle;
        }
    }

    public void TakeDamage(int damage, bool fromPlayer, ulong shooterCliendId)
    {
        currentHealth.Value -= damage;
    }

    private void LateUpdate() {
        rectTransform.rotation = Quaternion.Euler(0, 0,0);
        if (IsOwner){
        camera.transform.position = new Vector3(transform.position.x, transform.position.y, -10);        
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

    public void Fire(){
        m_Animator.SetTrigger("shootTrig");
        weapon.Fire(true);
        nextShot = Time.time + fireRate;
    }
}
