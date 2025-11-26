using UnityEngine;

public class MusicManager : MonoBehaviour {
    
    public static MusicManager Instance;
    
    void Awake() {
        // Singleton - hudba bude hrať cez všetky scény
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}
