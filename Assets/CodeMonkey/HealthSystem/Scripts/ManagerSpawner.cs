using UnityEngine;

public class ManagerSpawner : MonoBehaviour {
    
    [SerializeField] private GameObject deathCounterPrefab;
    
    void Awake() {
        // Spawn len raz na začiatku hry
        if (DeathCounter.Instance == null && deathCounterPrefab != null) {
            Instantiate(deathCounterPrefab);
            Debug.Log("DeathCounter prefab spawnutý!");
        }
    }
}
