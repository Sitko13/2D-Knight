using UnityEngine;
using CodeMonkey.HealthSystemCM;
using System.Collections.Generic;

public class SpiritExplosion : MonoBehaviour {
    
    [SerializeField] private int damage = 40;
    [SerializeField] private float explosionDuration = 2.5f;
    [SerializeField] private float maxScale = 6f;
    [SerializeField] private float scaleSpeed = 4f;
    
    private HashSet<GameObject> damagedObjects = new HashSet<GameObject>(); // Zoznam už poškodených
    private float timer = 0f;
    
    void Start() {
        transform.localScale = Vector3.zero;
        Destroy(gameObject, explosionDuration);
        
        Debug.Log("Spirit Explosion vytvorený na pozícii: " + transform.position);
    }
    
    void Update() {
        timer += Time.deltaTime;
        
        if (timer < explosionDuration * 0.7f) {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * maxScale, Time.deltaTime * scaleSpeed);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log("Spirit Explosion kolízia s: " + collision.gameObject.name + " (Tag: " + collision.tag + ")");
        
        // Kontrola či už nebol poškodený
        if (damagedObjects.Contains(collision.gameObject)) return;
        
        if (collision.CompareTag("Player")) {
            Debug.Log("Hráč detekovaný! Pokúšam sa poškodiť...");
            
            if (HealthSystem.TryGetHealthSystem(collision.gameObject, out HealthSystem healthSystem)) {
                healthSystem.Damage(damage);
                damagedObjects.Add(collision.gameObject);
                Debug.Log("Spirit Explosion zasiahla hráča! Damage: " + damage);
            } else {
                Debug.LogWarning("Hráč nemá HealthSystem!");
            }
        }
    }
    
    // Pre istotu aj OnTriggerStay2D
    private void OnTriggerStay2D(Collider2D collision) {
        if (damagedObjects.Contains(collision.gameObject)) return;
        
        if (collision.CompareTag("Player")) {
            if (HealthSystem.TryGetHealthSystem(collision.gameObject, out HealthSystem healthSystem)) {
                healthSystem.Damage(damage);
                damagedObjects.Add(collision.gameObject);
                Debug.Log("Spirit Explosion (Stay) zasiahla hráča! Damage: " + damage);
            }
        }
    }
}
