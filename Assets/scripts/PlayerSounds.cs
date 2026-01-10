using System.Collections;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [Header("Fontes de Audio")]
    public AudioSource mainSource;   // som principal (passos)

    bool wasPlaying = false;

    void Awake()
    {
        if (mainSource == null)
            mainSource = GetComponent<AudioSource>();
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
            mainSource.volume = 1f;
            if (!mainSource.isPlaying)
                mainSource.Play();
        }
        else // Se parou
        {
            if (wasPlaying)
            {
                if (mainSource.isPlaying)
                    mainSource.Stop();
            }
        }

        wasPlaying = isPlaying;
    }
}
