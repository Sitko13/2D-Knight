using UnityEngine;

public class BigBossController : MonoBehaviour {
    
    [Header("Fireball Attack")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireballInterval = 5f; // Každých 5 sekúnd
    
    [Header("Spirit Explosion Attack")]
    [SerializeField] private GameObject spiritExplosionPrefab;
    [SerializeField] private float spiritInterval = 10f; // Každých 10 sekúnd
    
    [Header("Fireball Settings")]
    [SerializeField] private bool shootAtPlayer = false;
    [SerializeField] private Vector2 fixedDirection = Vector2.left;
    
    private Animator animator;
    private float fireballTimer = 0f;
    private float spiritTimer = 0f;
    private Transform player;
    
    void Start() {
        animator = GetComponent<Animator>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) {
            player = playerObj.transform;
        }
        
        // Prvý fireball útok za 2 sekundy
        fireballTimer = fireballInterval - 2f;
        // Prvý spirit útok za 5 sekúnd
        spiritTimer = spiritInterval - 5f;
    }
    
    void Update() {
        // FIREBALL ÚTOK (každých 5 sekúnd)
        fireballTimer += Time.deltaTime;
        if (fireballTimer >= fireballInterval) {
            FireballAttack();
            fireballTimer = 0f;
        }
        
        // SPIRIT EXPLOSION ÚTOK (každých 10 sekúnd)
        spiritTimer += Time.deltaTime;
        if (spiritTimer >= spiritInterval) {
            SpiritExplosionAttack();
            spiritTimer = 0f;
        }
    }
    
    void FireballAttack() {
        Debug.Log("BIGBOSS: Fireball Attack!");
        
        // Spusti Attack trigger (ako na obrázku)
        if (animator != null) {
            animator.SetTrigger("Attack");
        }
        
        Invoke("ShootFireball", 0.5f);
    }
    
    void ShootFireball() {
        if (fireballPrefab == null || firePoint == null) {
            Debug.LogError("Fireball Prefab alebo Fire Point nie je nastavený!");
            return;
        }
        
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        Fireball fireballScript = fireball.GetComponent<Fireball>();
        
        if (fireballScript != null) {
            Vector2 direction;
            if (shootAtPlayer && player != null) {
                direction = (player.position - firePoint.position).normalized;
            } else {
                direction = fixedDirection;
            }
            fireballScript.SetDirection(direction);
        }
    }
    
    void SpiritExplosionAttack() {
        Debug.Log("BIGBOSS: Spirit Explosion Attack!");
        
        // Spusti Recover trigger (ako na obrázku)
        if (animator != null) {
            animator.SetTrigger("Recover");
        }
        
        // Vytvor výbuch po 0.8s (aby sa zosynchronizoval s Recover animáciou)
        Invoke("CreateSpiritExplosion", 2.0f);
    }
    
    void CreateSpiritExplosion() {
        if (spiritExplosionPrefab == null) {
            Debug.LogError("Spirit Explosion Prefab nie je nastavený!");
            return;
        }
        
        // Vytvor výbuch na pozícii bossa
        Instantiate(spiritExplosionPrefab, transform.position, Quaternion.identity);
    }
}
