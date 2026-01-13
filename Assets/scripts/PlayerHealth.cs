using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Valores de Vida")]
    public float maxHealth = 100f;
    public float currentHealth;

    public float regen = 3f;
    private float regenDelay = 3f;

    [Header("Movimento / Referencias")]
    public PlayerController playerController;
    public Slider healthBar;
    public DeathMenu deathMenu;

    public bool tookdamage = false;
    public bool isdead = false;

    // Timestamp of last time the player took damage
    private float lastDamageTime = -Mathf.Infinity;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    void Update()
    {
        UpdateUI();
        ManageHealth();
    }

    void PassiveRegen() 
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + regen * Time.deltaTime);
    }
    void ManageHealth() 
    {
        if (currentHealth > 0)
        {
            // Se regenar se ja passou regenDelay desde a ultima vez que tomou dano
            if (Time.time - lastDamageTime >= regenDelay)
            {
                PassiveRegen();
                tookdamage = false;
            }
            else
            {
                tookdamage = true;
            }
        }
        else currentHealth = 0;

    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        // registrar a hora do dano para atrasar a regeneracao
        lastDamageTime = Time.time;
        tookdamage = true;

        // limitar velocidade se stamina <= 0
        if (currentHealth <= 0f)
        {
            playerController.canWalk = false;
            isdead = true;
            Die();
        }
        else
        {
            playerController.canWalk = true;
        }
    }
    public void Die()
    {
        deathMenu.ShowDeathMenu();
    }
    void UpdateUI()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
            healthBar.fillRect.GetComponent<Image>().color = Color.Lerp(Color.black, Color.red, currentHealth / maxHealth);
        }
    }
}
