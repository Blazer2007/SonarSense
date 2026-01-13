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

    [Header("Stress Progressivo")]
    public float safeCrouchTime = 3f;    // tempo seguro em segundos
    public float baseCrouchCost = 0.5f;  // custo inicial
    public float maxCrouchCost = 3f;     // custo máximo
    public float crouchEscalationRate = 1f; // quão rápido o custo aumenta

    private float crouchTime;            // quanto tempo seguido está agachado

    [Header("Stun")]
    public float stunDuration = 2f;
    public float forcedStandTime = 3f;
    private bool isStunned;
    private float stunTimer;

    [Header("Movimento / Referências")]
    public PlayerController playerController;
    public PlayerStamina playerStamina;
    public Slider stressBar;               // arrasta o Slider da UI aqui
    public PlayerHealth playerHealth;

    public AudioSource audioSource;
    public AudioClip stressedClip;

    private bool isMoving;
    [SerializeField] private StunShake shake;


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
    }

    void Update()
    {
        HandleStress();
        HandleRegen();
        UpdateUI();
        HandleStun();

        playerController.isStressed = currentStress > 0f;

        float stressPercent = currentStress / maxStress;
        playerController.noiseLevel = playerController.baseNoise + playerController.maxExtraNoise * stressPercent;
    }

    void HandleStress()
    {
        if (playerController.isCrouching)
        {
            crouchTime += Time.deltaTime;

            // custo aumenta com o tempo agachado
            float t = Mathf.InverseLerp(safeCrouchTime, safeCrouchTime * 3f, crouchTime);
            float dynamicCost = Mathf.Lerp(baseCrouchCost, maxCrouchCost, t);

            Stress(dynamicCost * Time.deltaTime * 10f);
        }
        else
        {
            // reseta o tempo de agachamento quando levanta
            crouchTime = 0f;
        }
    }

    void HandleRegen()
    {
        isMoving = new Vector2(playerController.inputX, playerController.inputZ).sqrMagnitude > 0.01f;
        bool isCrouching = playerController.isCrouching;

        if (!isCrouching && isMoving && !isStunned)
        {
            currentStress = Mathf.Max(0, currentStress - regen * Time.deltaTime);
        }
        if (!isCrouching && !isMoving && !isStunned)
            currentStress = Mathf.Max(0, currentStress - (regen * 0.5f) * Time.deltaTime);

    }

    public void Stress(float amount)
    {
        currentStress = Mathf.Clamp(currentStress + amount, 0, maxStress);

        if (currentStress >= maxStress && !isStunned)
        {
            // força levantar

            playerController.isCrouching = false;
            playerController.canCrouch = false;
            playerController.canWalk = false;

            isStunned = true;
            stunTimer = stunDuration;

            playerHealth.TakeDamage(10); // dano ao ficar estressado
            HeavyBreath();

            
            if (shake != null)
            {
                shake.TriggerShake(1.5f, stunDuration);  // shake médio por 2 segundos
            }
        }
    }
    void HandleStun()
    {
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f)
            {
                isStunned = false;
                playerController.canWalk = true;
                playerController.canCrouch = true;
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
