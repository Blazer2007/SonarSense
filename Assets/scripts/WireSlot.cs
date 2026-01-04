using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WireSlot : MonoBehaviour, IDropHandler
{
    public int targetFrequency; // Frequencia de slot para encaixar o fio correspondente
    private PuzzleManager manager;
    private bool isOccupied = false; // Verifica se o slot já está ocupado
    public Material material;

    public Image slotImage; // Imagem do fio

    void Start()
    {
        manager = FindFirstObjectByType<PuzzleManager>();

        // Try to auto-assign image if not set in inspector
        if (slotImage == null)
            slotImage = GetComponent<Image>() ?? GetComponentInChildren<Image>();

        // Apply material if present
        if (material != null)
            ApplyMaterial(material);
    }

    public void SetMaterial(Material mat)
    {
        material = mat;
        ApplyMaterial(mat);
    }

    private void ApplyMaterial(Material mat)
    {
        if (slotImage != null)
        {
            // UI Image uses material on the graphic or a sprite; apply material if supported
            slotImage.material = mat;
            return;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (isOccupied) return; // Se o slot já estiver ocupado, o fio não encaixa

        WireDrag wire = eventData.pointerDrag?.GetComponent<WireDrag>(); // Tenta obter o componente WireDrag do objeto arrastado

        if (wire != null && Mathf.Approximately(wire.frequency, targetFrequency)) // Verifica se o fio corresponde à frequência do slot
        {
            wire.SnapToSlot(transform.position + new Vector3(-0.75f,0f,-0.5f)); // Encaixa o fio na posição do slot
            isOccupied = true; // Marca o slot como ocupado
            manager.currentConnections++; // Incrementa o valor de conexões atuais

            if (manager.currentConnections >= manager.connectionsNeeded) // Verifica se todas as conexões necessárias foram feitas
            {
                manager.OnPuzzleComplete(); // chama o método do puzzlemanager para fechar o puzzle
            }
        }
    }
}
