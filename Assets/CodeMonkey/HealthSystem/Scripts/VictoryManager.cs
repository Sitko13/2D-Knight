using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryManager : MonoBehaviour {
    
    public static VictoryManager Instance;
    
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private Text deathCountText;
    
    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }
    
    public void ShowVictory() {
        Debug.Log("VICTORY! Boss porazený!");
        
        if (victoryPanel != null) {
            victoryPanel.SetActive(true);
        }
        
        // Zobraz počet smrti
        if (deathCountText != null && DeathCounter.Instance != null) {
            int deaths = DeathCounter.Instance.GetDeathCount();
            
            if (deaths == 0) {
                deathCountText.text = "PERFEKTNÉ! Žiadne smrti!";
            } else if (deaths == 1) {
                deathCountText.text = "Počet pokusov: 1";
            } else {
                deathCountText.text = "Počet pokusov: " + deaths;
            }
        }
        
        // Vypni pohyb hráča
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) {
            MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
            foreach (var script in scripts) {
                if (script != null && !(script is Animator)) {
                    script.enabled = false;
                }
            }
            
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }
    }
    
    public void RestartLevel() {
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
