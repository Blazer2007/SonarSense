using System.Collections;
using UnityEngine;

public class PlayerEchoSounds : MonoBehaviour
{
    [Header("Fontes de Audio")]
    public AudioSource mainSource;   // som principal (passos)
    public AudioSource echoSource;   // eco

    [Header("Parâmetros de Eco")]
    [Range(0f, 1f)]
    public float echoVolume = 0.5f;
    public int echoRepeats = 3;
    public float echoDelay = 0.3f;
    public float echoClipDuration = 1f;

    bool wasPlaying = false;
    bool echoActive = false;
    float lastSecondStartTime = 0f; // Último segundo a ser repetido

    void Awake()
    {
        if (mainSource == null)
            mainSource = GetComponent<AudioSource>();

        if (echoSource == null)
        {
            echoSource = gameObject.AddComponent<AudioSource>();
            echoSource.spatialBlend = mainSource != null ? mainSource.spatialBlend : 1f;
            echoSource.playOnAwake = false;
        }
    }

    /// <summary>
    /// Chama isto a partir do controller de movimento:
    /// true  = jogador está a andar
    /// false = jogador parou
    /// </summary>
    public void UpdatePlayingState(bool isPlaying)
    {
        if (mainSource == null)
            return;

        // Se está a tocar
        if (isPlaying)
        {
            if (!mainSource.isPlaying)
                mainSource.Play();

            if (echoActive)
            {
                StopAllCoroutines();
                echoSource.Stop();
                echoActive = false;
            }
        }
        else // Se parou
        {
            if (wasPlaying)
            {
                if (mainSource.isPlaying)
                    mainSource.Stop();

                StartEcho();
            }
        }

        wasPlaying = isPlaying;
    }

    void StartEcho()
    {
        if (mainSource == null || mainSource.clip == null)
            return;

        echoSource.clip = mainSource.clip;
        echoSource.loop = false;

        float currentTime = mainSource.time;
        float clipLength = mainSource.clip.length;
        lastSecondStartTime = Mathf.Max(0f, currentTime - 1f);
        StopAllCoroutines();
        StartCoroutine(PlayEchoRepeats());
    }

    IEnumerator PlayEchoRepeats()
    {
        echoActive = true;
        float baseVolume = echoVolume;

        for (int i = 0; i < echoRepeats; i++)
        {
            echoSource.volume = baseVolume / Mathf.Pow(2f, i);

            // eco começa do último segundo do som principal
            echoSource.time = lastSecondStartTime;
            echoSource.Play();

            // Recolha do tempo restante no clip de eco
            float remaining = echoSource.clip.length - lastSecondStartTime;

            // Duração do eco nesta repetição
            float duration = Mathf.Min(echoClipDuration, remaining);

            // Esperar pela duração do eco
            yield return new WaitForSeconds(duration);

            // parar o som de eco
            echoSource.Stop();

            // Esperar pelo atraso antes da próxima repetição, caso não seja a última repetição
            if (i < echoRepeats-1)
                yield return new WaitForSeconds(echoDelay);
        }
    }
}
