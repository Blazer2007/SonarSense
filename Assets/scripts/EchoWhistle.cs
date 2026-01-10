using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class EchoWhistle : MonoBehaviour
{
    [Header("Player Hearing")]
    public float whistleRange = 15f; // Alcance do assobio

    [Header("Pulse Settings")]
    public float pulseSpeed = 15f;   // Ligeiramente mais lento que objetos
    public float pulseThickness = 1.2f; // Um pouco mais grosso
    public float cooldown = 2f;      // Tempo entre assobios

    [Header("Audio")]
    public AudioClip[] whistleSound;
    public AudioSource audioSource; // Fonte de áudio usada para tocar os clips

    private float currentDistance = 0f;
    private Vector3 pulseOrigin;
    private bool pulseActive = false;
    private float lastWhistleTime = 0f;

    [SerializeField] private PlayerStamina playerStamina;
    [SerializeField] private PlayerController playerController;

    // Shuffled play order
    private List<int> playOrder = new List<int>();
    private int playIndex = 0;

    void Start()
    {
        // Ensure we have an AudioSource
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Prepare shuffled order
        if (whistleSound != null && whistleSound.Length > 0)
            ShufflePlayOrder();
    }

    void Update()
    {
        // Assobio por tecla (W ou espaço) - para testes
        if (Input.GetKeyDown(KeyCode.Q) && !playerController.isFatigued)
        {
            Whistle();
        }

        if (!pulseActive) return;

        currentDistance += Time.deltaTime * pulseSpeed;
        Shader.SetGlobalFloat("_PulseDistance", currentDistance);

        // Fade out igual ao EchoPulse original
        float distToEdge = whistleRange + 15f; // buffer
        float fadeStart = distToEdge - 10f;
        float fade = 1f;

        if (currentDistance >= fadeStart)
        {
            float t = Mathf.InverseLerp(distToEdge, fadeStart, currentDistance);
            fade = Mathf.Clamp01(1f - t);
        }
        Shader.SetGlobalFloat("_PulseFade", fade);

        if (currentDistance >= distToEdge)
        {
            pulseActive = false;
            Shader.SetGlobalFloat("_PulseFade", 0f);
            Shader.SetGlobalFloat("_PulseDistance", 0f);
        }
    }

    public void Whistle()
    {
        if (Time.time < lastWhistleTime + cooldown) return;

        // Som do assobio - toca o próximo clip na ordem embaralhada
        if (whistleSound != null && whistleSound.Length > 0 && audioSource != null)
        {
            // Ensure playOrder is valid
            if (playOrder == null || playOrder.Count != whistleSound.Length)
                ShufflePlayOrder();

            AudioClip clip = whistleSound[playOrder[playIndex]];
            audioSource.PlayOneShot(clip);

            playIndex++;
            if (playIndex >= playOrder.Count)
                ShufflePlayOrder(); // reshuffle when exhausted
        }

        // Inicia o pulso do centro do jogador
        pulseOrigin = transform.position;
        Shader.SetGlobalVector("_PulseOrigin", pulseOrigin);
        Shader.SetGlobalFloat("_PulseDistance", 0f);
        Shader.SetGlobalFloat("_PulseTime", Time.time);
        Shader.SetGlobalFloat("_PulseThickness", pulseThickness);

        currentDistance = 0f;
        pulseActive = true;
        lastWhistleTime = Time.time;

        playerStamina.SpendStamina(10f); // Assobiar custa 10 de stamina 
    }

    private void ShufflePlayOrder()
    {
        playOrder = Enumerable.Range(0, whistleSound.Length).ToList();
        // Fisher-Yates shuffle
        for (int i = playOrder.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int tmp = playOrder[i];
            playOrder[i] = playOrder[j];
            playOrder[j] = tmp;
        }
        playIndex = 0;
    }
}
