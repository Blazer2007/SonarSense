using UnityEngine;
using UnityEngine.EventSystems;

public class WireDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public float frequency;
    private Vector2 startPos;
    private Canvas canvas;

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = transform.position;
        canvas = transform.parent.GetComponent<Canvas>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Volta se não encaixou (lógica no slot)
        transform.position = startPos;
    }
}
