using UnityEngine;
using System.Collections;
using CodeMonkey.HealthSystemCM;

public class HeroKnight : MonoBehaviour
{
    // === EXISTUJÚCE PRIVÁTNE A SERIALIZOVANÉ PREMENNÉ ===
    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;
    [SerializeField] float m_rollForce = 6.0f;
    [SerializeField] bool m_noBlood = false;
    [SerializeField] GameObject m_slideDust;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private Sensor_HeroKnight m_groundSensor;
    private Sensor_HeroKnight m_wallSensorR1;
    private Sensor_HeroKnight m_wallSensorR2;
    private Sensor_HeroKnight m_wallSensorL1;
    private Sensor_HeroKnight m_wallSensorL2;
    private bool m_isWallSliding = false;
    private bool m_grounded = false;
    private bool m_rolling = false;
    private int m_facingDirection = 1;
    private int m_currentAttack = 0;
    private float m_timeSinceAttack = 0.0f;
    private float m_delayToIdle = 0.0f;
    private float m_rollDuration = 8.0f / 14.0f;
    private float m_rollCurrentTime;

    // === ATTACK SETTINGS ===
    [Header("Attack Settings")]
    [Tooltip("Bod, z ktorého vychádza útok (prázdny GameObject).")]
    public Transform attackPoint;
    [Tooltip("Dosah (polomer) útoku. Viditeľné ako červený kruh v Editore.")]
    public float attackRange = 0.6f;
    [Tooltip("Poškodenie, ktoré hrdina spôsobí nepriateľovi.")]
    public int attackDamage = 20;
    [Tooltip("Vrstva, na ktorej sa nachádzajú nepriatelia.")]
    public LayerMask enemyLayers;

    // === SOUND SETTINGS ===
    [Header("Sound Settings")]
    [Tooltip("Zvuk pri útoku.")]
    [SerializeField] private AudioClip attackSound;
    private AudioSource audioSource;

    // === MOBILE CONTROLS ===
    [Header("Mobile Controls")]
    [Tooltip("Ak je true, použije mobile ovládanie (joystick + buttony)")]
    public bool useMobileControls = false;
    
    [Tooltip("Priradí Fixed Joystick alebo Floating Joystick z scény")]
    public FixedJoystick joystick; // Alebo použite FloatingJoystick
    
    private bool mobileJumpPressed = false;
    private bool mobileAttackPressed = false;
    private bool mobileRollPressed = false;

    // Use this for initialization
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
        
        // Získaj AudioSource komponent
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;

        // Increase timer that checks roll duration
        if (m_rolling)
            m_rollCurrentTime += Time.deltaTime;

        // Disable rolling if timer extends duration
        if (m_rollCurrentTime > m_rollDuration)
            m_rolling = false;

        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // === HANDLE INPUT (Klávesnica + Mobile) ===
        float inputX;
        
        if (useMobileControls)
        {
            // Mobile: použije joystick
            if (joystick != null)
            {
                inputX = joystick.Horizontal; // Vracia hodnotu od -1 (vľavo) do 1 (vpravo)
            }
            else
            {
                inputX = 0f;
                Debug.LogWarning("Joystick nie je priradený k HeroKnight!");
            }
        }
        else
        {
            // PC: klávesnica (A/D alebo šípky)
            inputX = Input.GetAxis("Horizontal");
        }

        // Swap direction of sprite depending on walk direction
        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }
        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }

        // Move
        if (!m_rolling)
            m_body2d.linearVelocity = new Vector2(inputX * m_speed, m_body2d.linearVelocity.y);

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.linearVelocity.y);

        // -- Handle Animations --
        //Wall Slide
        m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State());
        m_animator.SetBool("WallSlide", m_isWallSliding);

        //Death
        if (Input.GetKeyDown("l") && !m_rolling)
        {
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
        }

        //Hurt
        else if (Input.GetKeyDown("k") && !m_rolling)
            m_animator.SetTrigger("Hurt");

        // === ATTACK (Myš + Mobile button) ===
        bool attackInput = Input.GetMouseButtonDown(0) || mobileAttackPressed;
        
        if (attackInput && m_timeSinceAttack > 0.25f && !m_rolling)
        {
            m_currentAttack++;

            // Loop back to one after third attack
            if (m_currentAttack > 3)
                m_currentAttack = 1;

            // Reset Attack combo if time since last attack is too large
            if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            m_animator.SetTrigger("Attack" + m_currentAttack);

            // Prehraj attack zvuk
            PlayAttackSound();

            // Reset timer
            m_timeSinceAttack = 0.0f;
            
            // Reset mobile flag
            mobileAttackPressed = false;
        }

        // Block
        else if (Input.GetKeyDown("m") && !m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }
        else if (Input.GetMouseButtonUp(1))
            m_animator.SetBool("IdleBlock", false);

        // === ROLL (Shift + Mobile button) ===
        bool rollInput = Input.GetKeyDown("left shift") || mobileRollPressed;
        
        if (rollInput && !m_rolling && !m_isWallSliding)
        {
            m_rolling = true;
            m_animator.SetTrigger("Roll");
            m_rollCurrentTime = 0;
            m_body2d.linearVelocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.linearVelocity.y);
            
            // Reset mobile flag
            mobileRollPressed = false;
        }

        // === JUMP (Space + Mobile button) ===
        bool jumpInput = Input.GetKeyDown("space") || mobileJumpPressed;
        
        if (jumpInput && m_grounded && !m_rolling)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.linearVelocity = new Vector2(m_body2d.linearVelocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
            
            // Reset mobile flag
            mobileJumpPressed = false;
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
            if (m_delayToIdle < 0)
                m_animator.SetInteger("AnimState", 0);
        }
    }

    // ===============================================
    // ==== METÓDY PRE PREHRÁVANIE ZVUKU ====
    // ===============================================
    
    void PlayAttackSound()
    {
        if (attackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    // ===============================================
    // ==== PUBLIC METÓDY PRE MOBILE TLAČIDLÁ ====
    // ===============================================
    
    // Volá sa z JUMP buttonu (OnClick)
    public void OnJumpButton()
    {
        mobileJumpPressed = true;
    }
    
    // Volá sa z ATTACK buttonu (OnClick)
    public void OnAttackButton()
    {
        mobileAttackPressed = true;
    }
    
    // Volá sa z ROLL buttonu (voliteľné)
    public void OnRollButton()
    {
        mobileRollPressed = true;
    }

    // ===============================================
    // ==== METÓDA PRE SPÔSOBOVANIE POŠKODENIA (Animation Event) ====
    // ===============================================

    public void AttackDamage()
    {
        // Kontrola, či je AttackPoint nastavený v Inspectore
        if (attackPoint == null)
        {
            Debug.LogError("Attack Point nie je nastavený na hrdinovi v Inspectore!");
            return;
        }

        // Zisti všetkých nepriateľov v okruhu okolo AttackPoint
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Prejdi všetkých zasiahnutých nepriateľov
        foreach (Collider2D enemy in hitEnemies)
        {
            // Získaj HealthSystemComponent na zasiahnutom objekte
            HealthSystemComponent enemyHealthComp = enemy.GetComponent<HealthSystemComponent>();

            if (enemyHealthComp != null)
            {
                // Aplikuj poškodenie
                enemyHealthComp.GetHealthSystem().Damage(attackDamage);

                Debug.Log("Hrdina zasiahol nepriateľa: " + enemy.name + " za " + attackDamage + " damage");
            }
        }
    }

    // ===============================================
    // ==== POMOCNÁ METÓDA PRE VIZUALIZÁCIU ÚTOKU ====
    // ===============================================

    // Volané pre zobrazenie dosahu útoku v Editore (červený kruh)
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        // Vykresli červený kruh okolo AttackPoint
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    // ===============================================
    // ==== ANIMATION EVENTS (Existujúce) ====
    // ===============================================
    
    // Called in slide animation.
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (m_facingDirection == 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;

        if (m_slideDust != null)
        {
            // Set correct arrow spawn position
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }
}
