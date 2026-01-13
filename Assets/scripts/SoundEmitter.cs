using Unity.VisualScripting;
using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    private bool triggered = false;
    private bool inHand   = false;   // esta atualmente na mao do jogador?
    public ObjectSounds objectSounds;


    void OnCollisionEnter(Collision collision)
    {
        // se esta na mao, ignorar TODAS as colisoes
        if (inHand) return;

        // se ja emitiu neste lancamento, nao emitir outra vez
        if (triggered) return;

        triggered = true;

        EchoPulse pulse = FindFirstObjectByType<EchoPulse>();
        if (pulse != null)
            pulse.StartPulse(transform.position);

    }

    // chamado quando o jogador pega no objeto
    public void OnPickedUp()
    {
        inHand = true;
        triggered = false;
        objectSounds.PlayPickupSound();
    }

    // chamado quando o jogador larga/atira o objeto
    public void OnThrown()
    {
        inHand = false;
        // triggered continua false ate a primeira colisao depois do lancamento
        objectSounds.PlayThrowSound();
    }
}
