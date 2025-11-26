using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    
    public static GameManager Instance;
    
    [SerializeField] private GameObject gameOverPanel;
    
    void Awake() {
        // Singleton pattern
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }
    
    public void ShowGameOver() {
        Debug.Log("Game Over!");
        
        // Zastav hru
        Time.timeScale = 0f;
        
        // Zobraz Game Over panel
        if (gameOverPanel != null) {
            gameOverPanel.SetActive(true);
        }
    }
    
    public void RestartGame() {
        // Obnov čas
        Time.timeScale = 1f;
        
        // Reštartuj scénu
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void QuitGame() {
        Application.Quit();
        Debug.Log("Quit game");
    }
}
