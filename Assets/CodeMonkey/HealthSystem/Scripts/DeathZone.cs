using UnityEngine;
using CodeMonkey.HealthSystemCM;

public class DeathZone : MonoBehaviour {
    
    private void OnTriggerEnter2D(Collider2D collision) {
        // Skontroluj či objekt má tag "Player"
        if (collision.CompareTag("Player")) {
            Debug.Log("Player spadol do priepasti!");
            
            // Získaj HealthSystem a zabij hráča
            if (HealthSystem.TryGetHealthSystem(collision.gameObject, out HealthSystem healthSystem)) {
                healthSystem.Die(); // Priamo zabije hráča
            }
        }
    }
}
