using UnityEngine;

public class ThrowableEchoObject : MonoBehaviour
{
    public EchoSoundEmitter echoEmitter;

    bool isFlying = false;

    void Awake()
    {
        if (echoEmitter == null)
            echoEmitter = GetComponent<EchoSoundEmitter>();
    }

    public void OnThrown()
    {
        isFlying = true;
        echoEmitter.UpdatePlayingState(true);  // começou o som (quem decide tocar mainSource é o emissor)
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isFlying) return;

        isFlying = false;
        echoEmitter.UpdatePlayingState(false); // parou → eco do último segundo
    }
}
