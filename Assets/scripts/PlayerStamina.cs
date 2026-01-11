using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;   // se fores mostrar a barra aqui

public class PlayerStamina : MonoBehaviour
{
    [Header("Valores de Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float lastStamina;

    [Header("Custos / Regeneração")]
    public float costPerStep = 0.3f;          // custo de stamina ao andar
    public float regenCrouch = 7.5f;          // por segundo, agachado
    public float regenIdle = 12f;           // por segundo, parado de pé

    [Header("Movimento / Referências")]
    public PlayerController playerController;
    public Slider staminaBar;               // arrasta o Slider da UI aqui
    public PlayerHealth playerHealth;
    [SerializeField] EchoWhistle whistle;
    [SerializeField] PickUpThrow pickUpThrow;

    public AudioSource audioSource;
    public AudioClip[] coughingClip;

    private List<int> playOrder = new List<int>();
    private int playIndex = 0;

    public bool isMoving;

    // Tempo do último som de tosse
    private float lastCoughTime = -Mathf.Infinity;
    // Intervalo mínimo entre tosse em segundos
    public float coughInterval = 2.5f;

    void Start()
    {
        currentStamina = maxStamina;

        if (staminaBar != null)
        {
            staminaBar.maxValue = maxStamina;
            staminaBar.value = currentStamina;
        }
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (coughingClip != null && coughingClip.Length > 0)
            ShufflePlayOrder();
    }
    private void ShufflePlayOrder()
    {
        playOrder = Enumerable.Range(0, coughingClip.Length).ToList();
        
        for (int i = playOrder.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int tmp = playOrder[i];
            playOrder[i] = playOrder[j];
            playOrder[j] = tmp;
        }
        playIndex = 0;
    }

    void Update()
    {
        HandleMovementStamina();
        HandleRegen();
        UpdateUI();

        if (currentStamina < maxStamina * 0.2f) // menos de 20% de stamina
        {
            playerController.isFatigued = true;
            whistle.canwhistle = false;
            pickUpThrow.canThrow = false;
        }
        else
        {
            playerController.isFatigued = false;
            whistle.canwhistle = true;
            pickUpThrow.canThrow = true;
        }
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
            lastStamina = currentStamina;
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

            if (currentStamina <= 0)
            {
                Cough();
            }
        }
        else
        {
            playerController.canWalk = true;
        }
    }
    public void Cough()
    {
        if (Time.time < lastCoughTime + coughInterval) return;

        // Tossir - toca o próximo clip na ordem embaralhada
        if (coughingClip != null && coughingClip.Length > 0 && audioSource != null)
        {
            if (playOrder == null || playOrder.Count != coughingClip.Length)
                ShufflePlayOrder();

            AudioClip clip = coughingClip[playOrder[playIndex]];
            audioSource.PlayOneShot(clip);

            playIndex++;
            if (playIndex >= playOrder.Count)
                ShufflePlayOrder();
        }
        lastCoughTime = Time.time;
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
