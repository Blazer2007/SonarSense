using Unity.Cinemachine;
using UnityEngine;

public class StunShake : MonoBehaviour
{
    private CinemachineCamera vcam;
    private CinemachineBasicMultiChannelPerlin noise;

    void Awake()
    {
        vcam = GetComponent<CinemachineCamera>();
        noise = vcam.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }
    public void TriggerShake(float intensity, float duration)
    {
        if (noise == null) return;

        noise.AmplitudeGain = intensity;  // força do shake
        Invoke(nameof(StopShake), duration);
    }

    void StopShake()
    {
        noise.AmplitudeGain = 0f;
    }
}
