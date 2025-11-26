using UnityEngine;
using CodeMonkey.HealthSystemCM;

public class EnemyDeath : MonoBehaviour {
    
    private HealthSystem healthSystem;
    
    void Start() {
        // Získaj HealthSystem z tohto objektu
        if (HealthSystem.TryGetHealthSystem(gameObject, out HealthSystem hs)) {
            healthSystem = hs;
            healthSystem.OnDead += HealthSystem_OnDead;
        }
    }
    
    private void HealthSystem_OnDead(object sender, System.EventArgs e) {
        // Keď enemy zomrie, zničí sa
        Debug.Log(gameObject.name + " zomrel!");
        Destroy(gameObject);
    }
    
    void OnDestroy() {
        if (healthSystem != null) {
            healthSystem.OnDead -= HealthSystem_OnDead;
        }
    }
}
