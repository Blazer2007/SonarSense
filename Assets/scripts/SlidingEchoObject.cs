using UnityEngine;

public class SlidingEchoObject : MonoBehaviour
{
    public EchoSoundEmitter echoEmitter;
    public float slideSpeedThreshold = 0.1f;
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (echoEmitter == null)
            echoEmitter = GetComponent<EchoSoundEmitter>();
    }

    void Update()
    {
        bool isSliding = rb.linearVelocity.magnitude > slideSpeedThreshold;
        echoEmitter.UpdatePlayingState(isSliding);
    }
}
