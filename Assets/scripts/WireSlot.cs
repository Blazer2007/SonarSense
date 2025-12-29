using UnityEngine;
using UnityEngine.EventSystems;

public class WireSlot : MonoBehaviour, IDropHandler
{
    public float targetFrequency;
    private PuzzleManager manager;

    void Start() { manager = FindFirstObjectByType<PuzzleManager>(); }

    public void OnDrop(PointerEventData eventData)
    {
        WireDrag wire = eventData.pointerDrag?.GetComponent<WireDrag>();
        if (wire != null && Mathf.Approximately(wire.frequency, targetFrequency))
        {
            wire.transform.position = transform.position;
            manager.currentConnections++;
            if (manager.currentConnections >= manager.connectionsNeeded)
            {
                manager.OnPuzzleComplete();
            }
        }
    }
}
