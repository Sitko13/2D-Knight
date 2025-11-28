using UnityEngine;
using CodeMonkey.HealthSystemCM;

public class BossDeath : MonoBehaviour {
    
    private HealthSystem healthSystem;
    
    void Start() {
        if (HealthSystem.TryGetHealthSystem(gameObject, out HealthSystem hs)) {
            healthSystem = hs;
            healthSystem.OnDead += HealthSystem_OnDead;
            Debug.Log("BossDeath: HealthSystem pripojený na BIGBOSS!");
        } else {
            Debug.LogError("BossDeath: BIGBOSS nemá HealthSystem!");
        }
    }
    
    private void HealthSystem_OnDead(object sender, System.EventArgs e) {
        Debug.Log("========== BIGBOSS ZOMREL! ==========");
        
        // Zavolaj Victory screen
        if (VictoryManager.Instance != null) {
            Debug.Log("Zobrazujem Victory screen...");
            VictoryManager.Instance.ShowVictory();
        } else {
            Debug.LogError("CHYBA: VictoryManager.Instance neexistuje!");
        }
        
        // Vypni boss scripty
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (var script in scripts) {
            if (script != this && !(script is Animator)) {
                script.enabled = false;
            }
        }
    }
    
    void OnDestroy() {
        if (healthSystem != null) {
            healthSystem.OnDead -= HealthSystem_OnDead;
        }
    }
}
