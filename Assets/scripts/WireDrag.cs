using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WireDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public float frequency; // Frequência do fio
    public bool isConnected = false; // Verifica se o fio está conectado a um slot
    public bool isDragging = false;
    public bool starteddragging = false;
    private Vector3 startPos; // Posição inicial do fio
    private RectTransform rectTransform; // Referência ao RectTransform do fio
    private Canvas canvas; // Referência ao Canvas pai
    private CanvasGroup canvasGroup; // Para controlar a interatividade durante o drag
    public LineRenderer line; // Linha para esticar o fio
    public Transform fixedEnd; // Inicio fixo do fio

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        canvas = GetComponentInParent<Canvas>();
    }
    void Start()
    {
        // Inicializa a linha com 2 pontos: fixo + ponta de arrasto
        line.positionCount = 2;
        line.useWorldSpace = true;
        line.SetPosition(0, fixedEnd.position + new Vector3(0f,0f,-0.5f));
        line.SetPosition(1, rectTransform.position + new Vector3(-0.1f, 0f, -0.5f));
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isConnected) return; // não permite arrastar se já estiver conectado
        startPos = rectTransform.position;// Salva a posição inicial
        canvasGroup.blocksRaycasts = false;  // Permite drag suave
        isDragging = true;
        starteddragging = true;
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (isConnected) return;
        RectTransform parentRect = rectTransform.parent as RectTransform; // Obtém o RectTransform do pai(canvas)
        if (parentRect == null)
        {
            parentRect = canvas.transform as RectTransform; // se o pai for nulo, tenta usar o transform do canvas
            if (parentRect == null)
            {
                rectTransform.anchoredPosition += eventData.delta / Mathf.Max(1f, canvas.scaleFactor); // se for nulo, a posição do fio é utilizada diretamente
                return;
            }
        }

        // Escolhe a câmera correta: de preferencia a câmera do evento(puzzle_cam), senão a do canvas
        Camera cam = eventData.pressEventCamera != null ? eventData.pressEventCamera : (canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera);

        if (canvas.renderMode == RenderMode.WorldSpace) // mexer o fio em 3D
        {
            Vector3 worldPoint;

            // bool que verifica se o ponteiro foi convertido para o mundo 3D corretamente
            bool okWorld = RectTransformUtility.ScreenPointToWorldPointInRectangle(parentRect /*Rect transform atribuído anteriormente*/, 
                                                                                   eventData.position /* posição do ponteiro em pixels (screen space).*/, 
                                                                                   cam /*Camara atual*/, 
                                                                                   out worldPoint /*posição no mundo 3D*/); 

            if (okWorld) // se a conversão do ponteiro para o mundo 3D foi bem sucedida
            {
                rectTransform.position = worldPoint; // posiciona o fio na posição do mundo convertida
                line.SetPosition(1, rectTransform.position); // Atualiza a ponta da linha para seguir o fio
            }
            else // se a conversão falhou
            {
                rectTransform.anchoredPosition += eventData.delta / Mathf.Max(1f, canvas.scaleFactor); // move o fio baseado no ponteiro
                line.SetPosition(1, rectTransform.position); // Atualiza a ponta da linha para seguir o fio
            }
        }
        else // em 2D
        {
            // ScreenSpaceOverlay e ScreenSpaceCamera: trabalhar com coordenadas locais do pai(canvas)
            Vector2 localPoint;

            // Mesma logica de conversão do ponteiro, mas em vez de ser em 3D , é em 2D local
            bool okLocal = RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, cam, out localPoint /*Posição local*/);
            
            if (okLocal) // se a conversão foi bem sucedida
            {
                // Define a posição do fio para a posição local convertida
                rectTransform.anchoredPosition = localPoint;

                // Mantém o fio dentro dos limites do pai(canvas)
                Rect parentRectArea = parentRect.rect;
                Vector2 min = parentRectArea.min;
                Vector2 max = parentRectArea.max;
                Vector2 anchored = rectTransform.anchoredPosition;
                anchored.x = Mathf.Clamp(anchored.x, min.x, max.x);
                anchored.y = Mathf.Clamp(anchored.y, min.y, max.y);
                rectTransform.anchoredPosition = anchored;
                line.SetPosition(1, rectTransform.position); // Atualiza a ponta da linha para seguir o fio
            }
            else // se a conversão falhou
            {
                rectTransform.anchoredPosition += eventData.delta / Mathf.Max(1f, canvas.scaleFactor); // move o fio baseado no ponteiro
                line.SetPosition(1, rectTransform.position); // Atualiza a ponta da linha para seguir o fio
            }
            
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        isDragging = false;

        // Se não estiver conectado a um slot
        if (!isConnected)
        {
            // Volta à posição inicial se não encaixou
            rectTransform.anchoredPosition = startPos;
            line.SetPosition(1, rectTransform.position);
        }
        else return;
    }
    public void SnapToSlot(Vector3 slotPosition)
    {
        // Encaixa o fio na posição do slot
        rectTransform.position = slotPosition;
        // Marca o fio como conectado
        isConnected = true;

        rectTransform.position = slotPosition;
        line.SetPosition(1, slotPosition);
    }

}
