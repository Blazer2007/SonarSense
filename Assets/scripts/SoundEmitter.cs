using Unity.VisualScripting;
using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    private bool triggered = false;
    private bool inHand   = false;   // está atualmente na mão do jogador?
    public ObjectSounds objectSounds;


    void OnCollisionEnter(Collision collision)
    {
        // se está na mão, ignorar TODAS as colisões
        if (inHand) return;

        // se já emitiu neste lançamento, não emitir outra vez
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
        // triggered continua false até à primeira colisão depois do lançamento
        objectSounds.PlayThrowSound();
    }
}
