using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WireSlot : MonoBehaviour, IDropHandler
{
    public float targetFrequency; // Frequencia de slot para encaixar o fio correspondente
    private PuzzleManager manager;
    private bool isOccupied = false; // Verifica se o slot já está ocupado

    void Start()
    {
        manager = FindFirstObjectByType<PuzzleManager>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (isOccupied) return; // Se o slot já estiver ocupado, o fio não encaixa

        WireDrag wire = eventData.pointerDrag?.GetComponent<WireDrag>(); // Tenta obter o componente WireDrag do objeto arrastado

        if (wire != null && Mathf.Approximately(wire.frequency, targetFrequency)) // Verifica se o fio corresponde à frequência do slot
        {
            wire.SnapToSlot(transform.position); // Encaixa o fio na posição do slot
            isOccupied = true; // Marca o slot como ocupado
            manager.currentConnections++; // Incrementa o valor de conexões atuais

            Debug.Log($"Fio encaixado! Freq: {wire.frequency}"); 

            if (manager.currentConnections >= manager.connectionsNeeded) // Verifica se todas as conexões necessárias foram feitas
            {
                manager.OnPuzzleComplete(); // chama o método do puzzlemanager para fechar o puzzle
            }
        }
    }
}
