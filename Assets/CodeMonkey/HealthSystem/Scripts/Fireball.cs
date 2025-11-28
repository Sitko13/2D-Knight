using UnityEngine;
using CodeMonkey.HealthSystemCM;

public class Fireball : MonoBehaviour {
    
    [SerializeField] private float speed = 8f;
    [SerializeField] private int damage = 30;
    [SerializeField] private float lifetime = 2f; // Zničí sa po 5 sekundách
    
    private Vector2 direction;
    
    void Start() {
        // Zničí sa po určitom čase
        Destroy(gameObject, lifetime);
    }
    
    void Update() {
        // Pohyb guľe
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }
    
    public void SetDirection(Vector2 dir) {
        direction = dir.normalized;
        
        // Otočí sprite podľa smeru (voliteľné)
        if (dir.x < 0) {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision) {
        // Poškodí hráča
        if (collision.CompareTag("Player")) {
            if (HealthSystem.TryGetHealthSystem(collision.gameObject, out HealthSystem healthSystem)) {
                healthSystem.Damage(damage);
                Debug.Log("Fireball zasiahol hráča!");
            }
            Destroy(gameObject); // Guľa zmizne po zásahu
        }
        
        // Zničí sa pri náraze do zeme/steny
        if (collision.CompareTag("Ground") || collision.CompareTag("Wall")) {
            Destroy(gameObject);
        }
    }
}
