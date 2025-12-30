using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    public CinemachineCamera Puzzlecam; // câmera do puzzle
    public CinemachineCamera game3DCamera; // câmera 3D do jogo
    public GameObject[] wirePrefabs; // prefabs dos fios
    public Transform[] startPositions, slotPositions; // posições iniciais dos fios e slots
    public float[] frequencies; // Respetivas frequências dos fios/slots
    public int connectionsNeeded = 4; // número de conexões necessárias para completar o puzzle
    public int currentConnections = 0; // número atual de conexões feitas
    public PlayerController PlayerController; // script do jogador para impedir que ele se mova durante o puzzle
    public Canvas canvas; // canvas do puzzle

    public bool iswasActive = false; // verifica se o puzzle está/esteve ativo

    void Start()
    {
        // Atribui frequências aos slots automaticamente para que os fios respetivos se conectem
        List<float> slotFreqs = new List<float>(frequencies);

        foreach (Transform slot in slotPositions) // para cada slot em slotpositions(pai que guarda as posições dos slots)
        {
            float freq = slotFreqs[Random.Range(0, slotFreqs.Count)]; // armazena uma frequência aleatória da lista de frequências disponíveis
            slot.GetComponent<WireSlot>().targetFrequency = freq; // atribui a frequência ao slot atual
            slotFreqs.Remove(freq); // remove a frequência da lista para evitar repetições
        }
    }
    public void StartPuzzle()
    {
        Debug.Log("puzzle comecou");

        // Atribui que o puzzle está/esteve ativo(utilizado no PuzzleActivator para impedir que se faça o puzzle novamente)
        iswasActive = true;

        // Impede o movimento do jogador
        PlayerController.moveSpeed = 0f;
        PlayerController.jumpForce = 0f;

        // aumenta a prioridade da câmera do puzzle e diminui a camara 3D, fazendo uma transição entre as duas
        game3DCamera.Priority = 0;
        Puzzlecam.Priority = 100;

        // desbloqueia o cursor para que o jogador possa interagir com o puzzle
        Cursor.lockState = CursorLockMode.None;

        // Impede que o jogador faça som enquanto está no puzzle
        PlayerController.canplay = false;

        // 1. LIMPA fios antigos
        foreach (Transform pos in startPositions)
        {
            foreach (Transform child in pos) Destroy(child.gameObject);
        }

        // 2. CRIA lista de frequências PAREADAS para fios E slots
        List<float> freqList = new List<float>(frequencies); // [440,440,523,523]

        // 3. SPAWNA FIOS (já correto)
        for (int i = 0; i < startPositions.Length; i++)
        {
            Vector3 spawnPos = startPositions[i].position + new Vector3(0.5f, 0f, 0f);

            GameObject wire = Instantiate(wirePrefabs[Random.Range(0, wirePrefabs.Length)],
                                         spawnPos, Quaternion.identity, startPositions[i]);

            WireDrag wireDrag = wire.GetComponentInChildren<WireDrag>();
            float freq = freqList[Random.Range(0, freqList.Count)];
            if (wireDrag != null)
            {
                wireDrag.frequency = freq;
                freqList.Remove(freq);
            }
            else
            {
                Debug.LogError("ERRO: WireDrag não encontrado em: " + wire.name);
            }
        }

        // 4. ATRIBUI MESMAS FREQUÊNCIAS AOS SLOTS (NOVO!)
        freqList = new List<float>(frequencies); // Reinicia lista
        for (int i = 0; i < slotPositions.Length; i++)
        {
            float slotFreq = freqList[Random.Range(0, freqList.Count)];
            slotPositions[i].GetComponent<WireSlot>().targetFrequency = slotFreq;
            freqList.Remove(slotFreq);
            Debug.Log($"Slot {i}: {slotFreq}Hz");
        }
    }

    public void OnPuzzleComplete()
    {
        game3DCamera.Priority = 10;
        Puzzlecam.Priority = 0;

        Puzzlecam.gameObject.SetActive(false);
        game3DCamera.gameObject.SetActive(true);

        PlayerController.canplay = true;
        Cursor.lockState = CursorLockMode.Locked;

        PlayerController.moveSpeed = 5f;
        PlayerController.jumpForce = 5f;
        // Recompensa ou volta ao 3D
    }
}
