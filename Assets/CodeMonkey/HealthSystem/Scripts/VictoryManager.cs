using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // ← Zmeňte z TMPro na UnityEngine.UI

public class VictoryManager : MonoBehaviour {
    
    public static VictoryManager Instance;
    
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private Text deathCountText; // ← Zmeňte z TextMeshProUGUI na Text
    
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
    
    // DEBUG: Skontroluj či text existuje
    if (deathCountText == null) {
        Debug.LogError("deathCountText je NULL! Nie je priradený v Inspector!");
        return;
    }
    
    // DEBUG: Skontroluj či DeathCounter existuje
    if (DeathCounter.Instance == null) {
        Debug.LogError("DeathCounter.Instance je NULL! DeathCounter objekt neexistuje!");
        return;
    }
    
    // Zobraz počet smrti
    int deaths = DeathCounter.Instance.GetDeathCount();
    Debug.Log("Získaný počet smrti: " + deaths);
    
    if (deaths == 0) {
        deathCountText.text = "PERFEKTNÉ! Žiadne smrti!";
    } else if (deaths == 1) {
        deathCountText.text = "Počet pokusov: 1";
    } else {
        deathCountText.text = "Počet pokusov: " + deaths;
    }
    
    Debug.Log("Text nastavený na: " + deathCountText.text);
        
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
        
        if (DeathCounter.Instance != null) {
            DeathCounter.Instance.ResetDeathCount();
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void LoadMainMenu() {
        Time.timeScale = 1f;
        
        if (DeathCounter.Instance != null) {
            DeathCounter.Instance.ResetDeathCount();
        }
        
        SceneManager.LoadScene("MainMenu");
    }
}
