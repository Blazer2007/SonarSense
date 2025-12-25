using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WireController : MonoBehaviour
{
    public Transform startPin;   // pino fixo
    public Transform handle;     // ponta que o jogador arrasta
    [HideInInspector] public Transform connectedPin; // pino onde encaixou (pode ser null)

    private LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.useWorldSpace = true;
    }

    void Update()
    {
        if (startPin == null || handle == null) return;

        line.SetPosition(0, startPin.position);
        line.SetPosition(1, handle.position);
    }

    public void SnapToPin(Transform pin)
    {
        connectedPin = pin;
        handle.position = pin.position;
    }

    public void ResetToStart()
    {
        connectedPin = null;
        handle.position = startPin.position;
    }
}
