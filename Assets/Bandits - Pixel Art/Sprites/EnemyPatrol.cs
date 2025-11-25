using UnityEngine;
using CodeMonkey.HealthSystemCM; // DÔLEŽITÉ: Pre prístup k HealthSystem triedam

public class EnemyPatrol : MonoBehaviour
{
    // === Pohybové Premenné ===
    public GameObject pointA;
    public GameObject pointB;
    public float speed = 2f;

    // === Bojové Premenné ===
    [Header("Bojové Nastavenia")]
    public int attackDamage = 10; // Poškodenie, ktoré nepriateľ spôsobí
    public float attackCooldown = 1.0f; // Interval medzi poškodením (v sekundách)
    private float lastAttackTime = 0f; // Čas posledného útoku

    // === Interné Premenné ===
    private Rigidbody2D rb;
    private Transform currentPoint;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Začíname patrolovať smerom k bodu B
        currentPoint = pointB.transform;

        if (pointA == null || pointB == null)
        {
            Debug.LogError("Body A a B musia byť priradené v Inspectore!", gameObject);
        }
    }


    void Update()
    {
        HandlePatrolMovement();
    }


    private void HandlePatrolMovement()
    {
        // Pohyb smerom k aktuálnemu bodu
        if (currentPoint == pointB.transform)
        {
            // Pohyb doprava
            rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
        }
        else
        {
            // Pohyb doľava
            rb.linearVelocity = new Vector2(-speed, rb.linearVelocity.y);
        }

        // Zmeň smer, keď dosiahne blízko bodu
        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f)
        {
            if (currentPoint == pointB.transform)
            {
                currentPoint = pointA.transform;
            }
            else
            {
                currentPoint = pointB.transform;
            }
            Flip();
        }
    }


    void Flip()
    {
        // Otočenie smeru pohľadu (zmena škály X)
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }


    // Spustí sa, keď kolajder (s nastavením Is Trigger) zostane v kontakte s iným
    private void OnTriggerStay2D(Collider2D collision)
    {
        // 1. Kontrola cooldownu - útočíme len raz za attackCooldown
        if (Time.time > lastAttackTime + attackCooldown)
        {
            // 2. Hľadáme HealthSystemComponent na zasiahnutom objekte (hrdina)
            HealthSystemComponent targetHealthComp = collision.gameObject.GetComponent<HealthSystemComponent>();

            // 3. Ak objekt má HealthSystemComponent, spôsobte poškodenie
            if (targetHealthComp != null)
            {
                // Získame samotný HealthSystem
                HealthSystem healthSystem = targetHealthComp.GetHealthSystem();

                // Spôsobíme poškodenie volaním metódy Damage()
                healthSystem.Damage(attackDamage);

                // Aktualizujeme čas posledného útoku
                lastAttackTime = Time.time;

                Debug.Log("Hrdina bol zasiahnutý nepriateľom za: " + attackDamage);
            }
        }
    }
}