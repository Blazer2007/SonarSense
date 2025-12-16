using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class EchoSoundEmitter : MonoBehaviour
{
    public AudioSource mainSource;   // fonte principal do som
    public AudioSource echoSource;   // fonte do eco
    public float echoVolume = 0.3f;
    public float echoFadeTime = 2f;
    public AudioClip pickupSound;    // som ao pegar
    public AudioClip throwSound;     // som ao arremessar

    private bool wasPlaying;

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

    void Update()
    {
        bool isPlaying = mainSource.isPlaying;

        // transição "a tocar" -> "parou"
        Debug.Log("isplaying" + isPlaying);
        Debug.Log("wasplaying" + wasPlaying);
        if (!isPlaying && wasPlaying)
            StartEchoFromMain();

        wasPlaying = isPlaying;
        
    }

    public void StartEchoFromMain()
    {
        if (mainSource.clip == null) return;

        echoSource.clip = mainSource.clip;
        echoSource.time = mainSource.time;
        echoSource.volume = echoVolume;
        echoSource.loop = false;
        echoSource.Play();
        StopAllCoroutines();
        StartCoroutine(FadeOutEcho());
    }

    IEnumerator FadeOutEcho()
    {
        float startVolume = echoSource.volume;
        float elapsed = 0f;

        while (elapsed < echoFadeTime)
        {
            elapsed += Time.deltaTime;
            echoSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / echoFadeTime);
            yield return null;
        }

        echoSource.Stop();
        echoSource.volume = startVolume;
    }

    // Métodos de pegar e arremessar
    public void OnPickedUp()
    {
        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
    }

    public void OnThrown()
    {
        if (throwSound != null)
            AudioSource.PlayClipAtPoint(throwSound, transform.position);

        if (echoSource != null)
            StartEchoFromMain();

    }
}
