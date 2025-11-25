using UnityEngine;
using CodeMonkey.HealthSystemCM; // DÔLEITÉ pre HealthSystem

public class EnemyDamage : MonoBehaviour
{
    [Header("Bojové Nastavenia")]
    public int attackDamage = 10;
    public float attackCooldown = 1.0f;
    private float lastAttackTime = 0f;

    // Táto metóda sa spustí, keï sa kolajder s Is Trigger=true dotkne iného kolajdera
    private void OnTriggerStay2D(Collider2D collision)
    {
        // 1. Kontrola cooldownu - útoèíme len raz za attackCooldown
        if (Time.time > lastAttackTime + attackCooldown)
        {
            // 2. Skúste získa komponent HealthSystemComponent z objektu kolízie
            HealthSystemComponent targetHealthComp = collision.gameObject.GetComponent<HealthSystemComponent>();

            // 3. Ak objekt má HealthSystemComponent (napr. hrdina), spôsobte poškodenie
            if (targetHealthComp != null)
            {
                // Získame samotnı HealthSystem cez komponent
                HealthSystem healthSystem = targetHealthComp.GetHealthSystem();

                // POZOR: Pouívame metódu Damage()
                healthSystem.Damage(attackDamage);

                // Aktualizujeme èas posledného útoku
                lastAttackTime = Time.time;

                Debug.Log("Hrdina bol zasiahnutı nepriate¾om za: " + attackDamage);
            }
        }
    }
}