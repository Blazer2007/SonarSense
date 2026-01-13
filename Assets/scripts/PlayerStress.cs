using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;   // se fores mostrar a barra aqui

public class PlayerStress : MonoBehaviour
{
    [Header("Valores de Stress")]
    public float maxStress = 100f;
    public float currentStress;
    public float lastStress;

    [Header("Custos / Regeneração")]
    public float regen = 20f;          // Regeneração de stress ao andar
    public float CrouchCost = 0.5f;          // por segundo, agachado

    [Header("Movimento / Referências")]
    public PlayerController playerController;
    public PlayerStamina playerStamina;
    public Slider stressBar;               // arrasta o Slider da UI aqui
    public PlayerHealth playerHealth;

    public AudioSource audioSource;
    public AudioClip stressedClip;

    private TextMeshPro stressText;
    private bool isMoving;

    void Start()
    {
        currentStress = 0;

        if (stressBar != null)
        {
            stressBar.maxValue = maxStress;
            stressBar.value = currentStress;
        }
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }
        stressText = stressBar.GetComponentInChildren<TextMeshPro>();
    }

    void Update()
    {
        HandleStress();
        HandleRegen();
        UpdateUI();

        if (currentStress > 0f)
        {
            playerController.isStressed = true;
        }
        else
        {
            playerController.isStressed = false;
        }
    }

    void HandleStress()
    {
        if (playerController.isCrouching && playerStamina.currentStamina >= 20)
        {
            Stress(CrouchCost * Time.deltaTime * 10);
            stressText.ignoreVisibility = false;
        }
    }

    void HandleRegen()
    {
        isMoving = new Vector2(playerController.inputX, playerController.inputZ).sqrMagnitude > 0.01f;
        // define estes bools no PlayerController
        bool isCrouching = playerController.isCrouching;

        if (!isCrouching && isMoving)
        {
            currentStress = Mathf.Min(maxStress, currentStress - regen * Time.deltaTime);
        }
    }

    public void Stress(float amount)
    {
        currentStress = Mathf.Max(0, currentStress + amount);
        // limitar velocidade se stamina <= 0
        if (currentStress >= 100f)
        {
            playerController.canWalk = false;
            playerHealth.TakeDamage(5f * Time.deltaTime); // perder vida quando stamina esgotada

            if (currentStress > 0)
            {
                HeavyBreath();
            }
        }
    }
    public void HeavyBreath()
    {
        if (stressedClip != null && audioSource != null )
        {
            audioSource.clip = stressedClip;
            audioSource.Play();
        }
    }
    void UpdateUI()
    {
        if (stressBar != null)
        {
            stressBar.value = currentStress;
        }
    }
}
