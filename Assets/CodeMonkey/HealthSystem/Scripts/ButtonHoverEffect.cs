using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    
    [SerializeField] private float scaleAmount = 1.1f;
    [SerializeField] private float animationSpeed = 10f;
    
    private Vector3 originalScale;
    private Vector3 targetScale;
    
    void Start() {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }
    
    void Update() {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * animationSpeed);
    }
    
    public void OnPointerEnter(PointerEventData eventData) {
        targetScale = originalScale * scaleAmount;
    }
    
    public void OnPointerExit(PointerEventData eventData) {
        targetScale = originalScale;
    }
}
