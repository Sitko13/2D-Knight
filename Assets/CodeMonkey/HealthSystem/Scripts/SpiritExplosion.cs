using UnityEngine;
using CodeMonkey.HealthSystemCM;

public class SpiritExplosion : MonoBehaviour {
    
    [SerializeField] private int damage = 40;
    [SerializeField] private float explosionDuration = 1f; // Ako dlho trvá výbuch
    [SerializeField] private float scaleSpeed = 5f; // Rýchlosť zväčšovania
    
    private bool hasExploded = false;
    private float timer = 0f;
    
    void Start() {
        // Začne malý, potom sa zväčší
        transform.localScale = Vector3.zero;
        
        // Zničí sa po dobe trvania
        Destroy(gameObject, explosionDuration);
    }
    
    void Update() {
        timer += Time.deltaTime;
        
        // Zväčšovanie efektu (expanduje výbuch)
        if (timer < explosionDuration * 0.7f) {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 5f, Time.deltaTime * scaleSpeed);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision) {
        if (hasExploded) return;
        
        // Poškodí hráča
        if (collision.CompareTag("Player")) {
            if (HealthSystem.TryGetHealthSystem(collision.gameObject, out HealthSystem healthSystem)) {
                healthSystem.Damage(damage);
                Debug.Log("Spirit Explosion zasiahla hráča! Damage: " + damage);
                hasExploded = true;
            }
        }
    }
}
