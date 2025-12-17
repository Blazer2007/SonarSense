using System.Collections;
using UnityEngine;

public class EchoSoundEmitter : MonoBehaviour
{
    public AudioSource mainSource;   // som principal
    public AudioSource echoSource;   // eco

    public float echoVolume = 0.5f;
    public int echoRepeats = 3;
    public float echoDelay = 1f;
    public float echoClipDuration = 1f;

    bool wasPlaying = false;
    bool echoActive = false;
    float lastSecondStartTime = 0f;

    void Awake()
    {
        if (mainSource == null)
            mainSource = GetComponent<AudioSource>();

        if (echoSource == null)
        {
            echoSource = gameObject.AddComponent<AudioSource>();
            echoSource.spatialBlend = mainSource.spatialBlend;
        }
    }

    // GENÉRICO: qualquer objeto chama isto com "true = está a emitir som"
    public void UpdatePlayingState(bool isPlaying)
    {
        // Estado "a tocar som principal"
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
        else
        {
            // transição: estava a tocar e agora parou → dispara eco
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

            echoSource.time = lastSecondStartTime;
            echoSource.Play();

            float remaining = echoSource.clip.length - lastSecondStartTime;
            float duration = Mathf.Min(echoClipDuration, remaining);

            yield return new WaitForSeconds(duration);

            echoSource.Stop();

            if (i < echoRepeats - 1)
                yield return new WaitForSeconds(echoDelay);
        }

        echoSource.Stop();
        echoActive = false;
    }
}
