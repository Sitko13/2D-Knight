using UnityEngine;

public class BigBossController : MonoBehaviour {
    
    [Header("Attack Settings")]
    [SerializeField] private GameObject fireballPrefab; // Prefab ohnivej gule
    [SerializeField] private Transform firePoint; // Bod odkiaľ vyletí guľa
    [SerializeField] private float attackInterval = 5f; // Každých 5 sekúnd
    
    [Header("Fireball Settings")]
    [SerializeField] private bool shootAtPlayer = true; // Ak true, strieľa na hráča
    [SerializeField] private Vector2 fixedDirection = Vector2.left; // Ak shootAtPlayer = false
    
    private Animator animator;
    private float attackTimer = 0f;
    private Transform player;
    
    void Start() {
        animator = GetComponent<Animator>();
        
        // Nájdi hráča
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) {
            player = playerObj.transform;
        }
        
        // Prvý útok za 2 sekundy
        attackTimer = attackInterval - 2f;
    }
    
    void Update() {
        attackTimer += Time.deltaTime;
        
        // Každých 5 sekúnd útok
        if (attackTimer >= attackInterval) {
            Attack();
            attackTimer = 0f;
        }
    }
    
    void Attack() {
        Debug.Log("BIGBOSS útočí!");
        
        // Spusti attack animáciu (ak máte)
        if (animator != null) {
            animator.SetTrigger("Attack");
        }
        
        // Vystreľ guľu po 0.5s (aby sa zosynchronizovala s animáciou)
        Invoke("ShootFireball", 0.5f);
    }
    
    void ShootFireball() {
        if (fireballPrefab == null || firePoint == null) {
            Debug.LogError("Fireball Prefab alebo Fire Point nie je nastavený!");
            return;
        }
        
        // Vytvor ohnivú guľu
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        Fireball fireballScript = fireball.GetComponent<Fireball>();
        
        if (fireballScript != null) {
            Vector2 direction;
            
            // Strieľa smer k hráčovi alebo fixným smerom
            if (shootAtPlayer && player != null) {
                direction = (player.position - firePoint.position).normalized;
            } else {
                direction = fixedDirection;
            }
            
            fireballScript.SetDirection(direction);
        }
    }
}
