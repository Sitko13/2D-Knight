using UnityEngine;

public class MusicManager : MonoBehaviour {
    
    public static MusicManager Instance;
    private AudioSource audioSource;
    
    void Awake() {
        // Singleton - hudba bude hrať cez všetky scény
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        
        // Získaj AudioSource
        audioSource = GetComponent<AudioSource>();
    }
    
    void Update() {
        // Zabezpeč že hudba hrá aj keď je Time.timeScale = 0 (Game Over)
        if (Time.timeScale == 0f && audioSource != null) {
            if (!audioSource.isPlaying) {
                audioSource.UnPause();
            }
        }
    }
}
