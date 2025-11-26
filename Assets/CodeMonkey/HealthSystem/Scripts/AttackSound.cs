using UnityEngine;

public class AttackSound : MonoBehaviour {
    
    [SerializeField] private AudioClip attackSound;
    private AudioSource audioSource;
    
    void Start() {
        audioSource = GetComponent<AudioSource>();
    }
    
    // Túto funkciu zavoláte keď hráč útočí
    public void PlayAttackSound() {
        if (attackSound != null && audioSource != null) {
            audioSource.PlayOneShot(attackSound);
        }
    }
}
