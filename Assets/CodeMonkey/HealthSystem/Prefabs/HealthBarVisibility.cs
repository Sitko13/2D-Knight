using UnityEngine;
using CodeMonkey.HealthSystemCM;
using UnityEngine.UI; // Pre Image komponent, ak chceme priamo kontrolovaù plnosù

public class HealthBarVisibility : MonoBehaviour
{
    // Odkaz na hlavn˝ Canvas komponent Health Baru
    private Canvas healthBarCanvas;

    // Odkaz na HealthSystem komponent nepriateæa
    private HealthSystem healthSystem;

    // Maxim·lna hodnota zdravia pri ötarte (NepotrebnÈ, ale ponech·me pre kontext)
    // private float initialHealth;

    private void Awake()
    {
        // ZÌskaj referenciu na Canvas (alebo GameObject) tohto Health Baru
        healthBarCanvas = GetComponent<Canvas>();

        // Na zaËiatku skryjeme Canvas
        if (healthBarCanvas != null)
        {
            healthBarCanvas.enabled = false;
        }
    }

    private void Start()
    {
        // AlternatÌvny spÙsob, ako zÌskaù HealthSystem, ak HealthBarUI.GetHealthSystemGameObject() nefunguje.
        // Predpoklad·me, ûe Health Bar je dieùaùom objektu s HealthSystemComponent, alebo je prepojen˝.

        // 1. Sk˙sime zÌskaù HealthSystemComponent z Parent objektu
        HealthSystemComponent healthSystemComp = GetComponentInParent<HealthSystemComponent>();

        // 2. Ak Health Bar sl˙ûi pre seba, sk˙sime na Úom
        if (healthSystemComp == null)
        {
            healthSystemComp = GetComponent<HealthSystemComponent>();
        }

        if (healthSystemComp != null)
        {
            healthSystem = healthSystemComp.GetHealthSystem();

            // Prihl·s sa k udalosti zmeny zdravia
            healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;

            // Skry Health Bar, ak je plnÈ zdravie
            UpdateVisibility();
        }
        else
        {
            Debug.LogError("HealthSystemComponent nebol n·jden˝ na rodiËovskom objekte (alebo Health Bar objekte) pre HealthBarVisibility!");
        }
    }

    // Pomocn· funkcia na kontrolu viditeænosti (vol· sa pri ötarte a pri zmene zdravia)
    private void UpdateVisibility()
    {
        if (healthSystem == null || healthBarCanvas == null) return;

        // Zobraziù Health Bar, ak nie je plnÈ zdravie (menej ako 100%)
        // PouûÌvame GetHealthNormalized() < 1f namiesto IsFullHealth()
        bool shouldBeVisible = healthSystem.GetHealthNormalized() < 1f;

        healthBarCanvas.enabled = shouldBeVisible;
    }


    private void HealthSystem_OnHealthChanged(object sender, System.EventArgs e)
    {
        // Pri kaûdej zmene zdravia aktualizujeme viditeænosù
        UpdateVisibility();
    }

    // Uvoænenie udalosti, keÔ sa objekt zniËÌ
    private void OnDestroy()
    {
        if (healthSystem != null)
        {
            healthSystem.OnHealthChanged -= HealthSystem_OnHealthChanged;
        }
    }
}