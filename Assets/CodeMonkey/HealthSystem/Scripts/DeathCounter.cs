using UnityEngine;

public class DeathCounter : MonoBehaviour {
    
    public static DeathCounter Instance;
    
    private int deathCount = 0;
    
    void Awake() {
        // Singleton - prežije reštarty
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
    
    public void AddDeath() {
        deathCount++;
        Debug.Log("Počet smrti: " + deathCount);
    }
    
    public int GetDeathCount() {
        return deathCount;
    }
    
    public void ResetDeathCount() {
        deathCount = 0;
    }
}
