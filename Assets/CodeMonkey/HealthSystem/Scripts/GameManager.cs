using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    
    public static GameManager Instance;
    
    [SerializeField] private GameObject gameOverPanel;
    private bool isGameOver = false;
    
    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }
    
    public void ShowGameOver() {
        if (isGameOver) return;
        isGameOver = true;
        
        Debug.Log("Game Over!");
        
        // NOVÉ: Pripočítaj smrť
        if (DeathCounter.Instance != null) {
            DeathCounter.Instance.AddDeath();
        }
        
        if (gameOverPanel != null) {
            gameOverPanel.SetActive(true);
        }
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) {
            MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
            foreach (var script in scripts) {
                if (!(script is Animator)) {
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
    
    public void RestartGame() {
        Time.timeScale = 1f;
        isGameOver = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void QuitGame() {
        Time.timeScale = 1f;
        Application.Quit();
        Debug.Log("Quit game");
    }
}
