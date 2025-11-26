using UnityEngine;
using CodeMonkey.HealthSystemCM;

public class PlayerDeath : MonoBehaviour {
    
    private HealthSystem healthSystem;
    private Animator animator;
    
    void Start() {
        animator = GetComponent<Animator>();
        
        if (HealthSystem.TryGetHealthSystem(gameObject, out HealthSystem hs)) {
            healthSystem = hs;
            healthSystem.OnDead += HealthSystem_OnDead;
        }
    }
    
    private void HealthSystem_OnDead(object sender, System.EventArgs e) {
        Debug.Log("Player zomrel!");
        
        // Vypni movement
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (var script in scripts) {
            if (script != this && !(script is Animator)) {
                script.enabled = false;
            }
        }
        
        // Volaj a Game Over
        if (GameManager.Instance != null) {
            GameManager.Instance.ShowGameOver();
        }
    }
    
    void OnDestroy() {
        if (healthSystem != null) {
            healthSystem.OnDead -= HealthSystem_OnDead;
        }
    }
}
