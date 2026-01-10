using UnityEngine;
using UnityEngine.UI;   // se fores mostrar a barra aqui

public class PlayerStamina : MonoBehaviour
{
    [Header("Valores de Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;

    [Header("Custos / Regeneração")]
    public float costPerStep = 0.3f;          // custo de stamina ao andar
    public float regenCrouch = 7.5f;          // por segundo, agachado
    public float regenIdle = 12f;           // por segundo, parado de pé

    [Header("Movimento / Referências")]
    public PlayerController playerController;
    public Slider staminaBar;               // arrasta o Slider da UI aqui
    public PlayerHealth playerHealth;

    public AudioSource audioSource;
    public AudioClip coughingClip;
    public bool isMoving;

    // Tempo do último som de tosse
    private float lastCoughTime = -Mathf.Infinity;
    // Intervalo mínimo entre tosse em segundos
    public float coughInterval = 2f;

    void Start()
    {
        currentStamina = maxStamina;

        if (staminaBar != null)
        {
            staminaBar.maxValue = maxStamina;
            staminaBar.value = currentStamina;
        }
    }

    void Update()
    {
        HandleMovementStamina();
        HandleRegen();
        UpdateUI();
    }

    void HandleMovementStamina()
    {
        if (isMoving && !playerController.isCrouching)
        {
            SpendStamina(costPerStep * Time.deltaTime * 20);
        }
    }

    void HandleRegen()
    {
        isMoving = new Vector2(playerController.inputX, playerController.inputZ).sqrMagnitude > 0.01f;
        // define estes bools no PlayerController
        bool isCrouching = playerController.isCrouching;

        if (!isMoving)
        {
            // parado: regenera mais rápido
            currentStamina = Mathf.Min(maxStamina, currentStamina + regenIdle * Time.deltaTime);
        }
        else if (isCrouching)
        {
            // a andar agachado: regenera devagar
            currentStamina = Mathf.Min(maxStamina, currentStamina + regenCrouch * 0.5f * Time.deltaTime);
        }
    }

    public void SpendStamina(float amount)
    {
        currentStamina = Mathf.Max(0, currentStamina - amount);
        // limitar velocidade se stamina <= 0
        if (currentStamina <= 0f)
        {
            playerController.canWalk = false;
            playerHealth.TakeDamage(5f * Time.deltaTime); // perder vida quando stamina esgotada

            // Toca o som de tosse no máximo uma vez por coughInterval segundos
            if (audioSource != null && coughingClip != null)
            {
                if (Time.time >= lastCoughTime + coughInterval)
                {
                    audioSource.PlayOneShot(coughingClip);
                    lastCoughTime = Time.time;
                }
            }
        }
        else
        {
            playerController.canWalk = true;
            if (currentStamina < maxStamina * 0.2f) // menos de 20% de stamina
            {
                playerController.isFatigued = true;
            }
            else
            {
                playerController.isFatigued = false;
            }
        }
    }

    void UpdateUI()
    {
        if (staminaBar != null)
        {
            staminaBar.value = currentStamina;

            if (staminaBar.value < 60 && staminaBar.value > 30)
                staminaBar.fillRect.GetComponent<Image>().color = Color.yellow;
            else if (staminaBar.value <= 30)
                staminaBar.fillRect.GetComponent<Image>().color = Color.red;
            else
                staminaBar.fillRect.GetComponent<Image>().color = Color.green;

        }
    }
}
