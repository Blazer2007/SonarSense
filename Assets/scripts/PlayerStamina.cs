using UnityEngine;
using UnityEngine.UI;   // se fores mostrar a barra aqui

public class PlayerStamina : MonoBehaviour
{
    [Header("Valores de Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;

    [Header("Custos / Regeneração")]
    public float costPerStep = 0.5f;        // quanto tira por “passo”
    public float regenCrouch = 5f;          // por segundo, agachado
    public float regenIdle = 10f;           // por segundo, parado de pé

    [Header("Movimento / Referências")]
    public PlayerController playerController;
    public Slider staminaBar;               // arrasta o Slider da UI aqui

    public bool isMoving;

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
        if (isMoving)
        {
                SpendStamina(costPerStep);
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

    void SpendStamina(float amount)
    {
        currentStamina = Mathf.Max(0, currentStamina - amount);
        // limitar velocidade se stamina <= 0
        if (currentStamina <= 0f)
        {
            playerController.canWalk = false;
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
        }
    }
}
