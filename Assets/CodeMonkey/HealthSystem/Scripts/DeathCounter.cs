using UnityEngine;
using UnityEngine.UI;

public class DeathCounter : MonoBehaviour {

    public static DeathCounter Instance;

    private int deathCount = 0;
    [SerializeField] private Text deathDisplayText;

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);      // Znič nové kópie po loade scény
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Tento jeden objekt prežije všetky loady
    }

    void Start() {
        UpdateDisplay();
    }

    public void AddDeath() {
        deathCount++;
        UpdateDisplay();
    }

    public int GetDeathCount() {
        return deathCount;
    }

    private void UpdateDisplay() {
        if (deathDisplayText != null) {
            deathDisplayText.text = "Deaths: " + deathCount;
        }
    }
}
