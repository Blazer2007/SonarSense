using UnityEngine;
using UnityEngine.EventSystems;

public class WireHandleUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public RectTransform handleRect;
    public WireController wire;
    public float snapRadius = 50f; // em pixels no Canvas

    private bool isDragging = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;

        // Verifica se está perto de um pino de destino
        GameObject[] pins = GameObject.FindGameObjectsWithTag("EndPin");
        RectTransform closest = null;
        float closestDist = Mathf.Infinity;

        foreach (var p in pins)
        {
            RectTransform pinRect = p.GetComponent<RectTransform>();
            float d = Vector2.Distance(handleRect.anchoredPosition, pinRect.anchoredPosition);
            if (d < closestDist && d <= snapRadius)
            {
                closestDist = d;
                closest = pinRect;
            }
        }

        if (closest != null)
        {
            handleRect.anchoredPosition = closest.anchoredPosition;
            // atualiza o WireController
            wire.SnapToPin(closest.transform);
        }
        else
        {
            handleRect.anchoredPosition = wire.startPin.GetComponent<RectTransform>().anchoredPosition;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        // Move a posição do handle conforme o rato/toque
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            handleRect.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPos
        );

        handleRect.anchoredPosition = localPos;
    }
}
