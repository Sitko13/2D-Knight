using UnityEngine;

using System.Collections;

using CodeMonkey.HealthSystemCM; // DÔLEŽITÉ: Musí byť pridané pre prístup k HealthSystem



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





    // === NOVÉ PREMENNÉ PRE ÚTOK HRDINU ===

    [Header("Attack Settings")]

    [Tooltip("Bod, z ktorého vychádza útok (prázdny GameObject).")]

    public Transform attackPoint;

    [Tooltip("Dosah (polomer) útoku. Viditeľné ako červený kruh v Editore.")]

    public float attackRange = 0.6f;

    [Tooltip("Poškodenie, ktoré hrdina spôsobí nepriateľovi.")]

    public int attackDamage = 20;

    [Tooltip("Vrstva, na ktorej sa nachádzajú nepriatelia.")]

    public LayerMask enemyLayers;





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



        // -- Handle input and movement --

        float inputX = Input.GetAxis("Horizontal");



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

        if (Input.GetKeyDown("e") && !m_rolling)

        {

            m_animator.SetBool("noBlood", m_noBlood);

            m_animator.SetTrigger("Death");

        }



        //Hurt

        else if (Input.GetKeyDown("q") && !m_rolling)

            m_animator.SetTrigger("Hurt");



        //Attack

        // Útočí, keď klikneme myšou, nie je v rolle a od posledného útoku prešlo 0.25 sekundy

        else if (Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !m_rolling)

        {

            // LOG 1: Kontrola, či sa vôbec dostaneme do sekcie útoku (malo by sa objaviť v konzole po každom kliknutí)

            Debug.Log("LOG 1: Útok spustil Animáciu.");



            m_currentAttack++;



            // Loop back to one after third attack

            if (m_currentAttack > 3)

                m_currentAttack = 1;



            // Reset Attack combo if time since last attack is too large

            if (m_timeSinceAttack > 1.0f)

                m_currentAttack = 1;



            // Call one of three attack animations "Attack1", "Attack2", "Attack3"

            m_animator.SetTrigger("Attack" + m_currentAttack);



            // Reset timer

            m_timeSinceAttack = 0.0f;

        }



        // Block

        else if (Input.GetMouseButtonDown(1) && !m_rolling)

        {

            m_animator.SetTrigger("Block");

            m_animator.SetBool("IdleBlock", true);

        }



        else if (Input.GetMouseButtonUp(1))

            m_animator.SetBool("IdleBlock", false);



        // Roll

        else if (Input.GetKeyDown("left shift") && !m_rolling && !m_isWallSliding)

        {

            m_rolling = true;

            m_animator.SetTrigger("Roll");

            m_rollCurrentTime = 0; // Reset roll timer

            m_body2d.linearVelocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.linearVelocity.y);

        }





        //Jump

        else if (Input.GetKeyDown("space") && m_grounded && !m_rolling)

        {

            m_animator.SetTrigger("Jump");

            m_grounded = false;

            m_animator.SetBool("Grounded", m_grounded);

            m_body2d.linearVelocity = new Vector2(m_body2d.linearVelocity.x, m_jumpForce);

            m_groundSensor.Disable(0.2f);

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

    // ==== METÓDA PRE SPÔSOBOVANIE POŠKODENIA (Volaná cez Animation Event) ====

    // ===============================================



    public void AttackDamage()

    {

        // LOG 2: Kontrola, či sa táto funkcia vôbec zavolala (z Animation Eventu)

        Debug.Log("LOG 2: Animation Event bol úspešne spustený. Hľadáme nepriateľov...");



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



                // LOG 3: Kontrola úspešného zásahu (ak sa táto správa objaví, útok funguje!)

                Debug.Log("LOG 3: Hrdina zasiahol nepriateľa: " + enemy.name + " za " + attackDamage);

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





    // Animation Events (Existujúce)

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