using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    public bool puzzledoor = false;
    public CinemachineCamera Puzzlecam;
    public CinemachineCamera game3DCamera;
    public GameObject[] wirePrefabs;
    public Transform[] startPositions, slotPositions;
    public int[] frequencies;              // ex: {1,2,3}
    public int connectionsNeeded = 3;
    public int currentConnections = 0;
    public PlayerController PlayerController;
    public Canvas canvas;
    public Material[] wireMaterials;       // mesmo tamanho que frequencies

    [SerializeField] private PlayerStamina playerStamina;
    [SerializeField] private Animator animator;

    public bool isActive = false;
    public bool wasActive = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        // Frequências únicas para os slots
        List<int> slotFreqs = new List<int>(frequencies);
        slotFreqs.Shuffle();

        int slotCount = Mathf.Min(slotPositions.Length, slotFreqs.Count);

        for (int i = 0; i < slotCount; i++)
        {
            int freq = slotFreqs[i];
            WireSlot slot = slotPositions[i].GetComponent<WireSlot>();

            slot.targetFrequency = freq;
            int matIndex = System.Array.IndexOf(frequencies, freq);
            slot.slotImage.material = wireMaterials[matIndex];
        }
    }
    public void Update()
    {
        if (isActive && !wasActive)
        {
            playerStamina.currentStamina = playerStamina.lastStamina;
            PlayerController.isMoving = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            PlayerController.moveSpeed = 0f;
            PlayerController.canplay = false;
        }
    }
    public void StartPuzzle()
    {
        isActive = true;

        PlayerController.moveSpeed = 0f;
        PlayerController.jumpForce = 0f;
        PlayerController.canplay = false;

        game3DCamera.Priority = 0;
        Puzzlecam.Priority = 100;

        Cursor.lockState = CursorLockMode.None;

        // Frequências únicas para os fios
        List<int> freqList = new List<int>(frequencies);
        freqList.Shuffle();

        int wireCount = Mathf.Min(startPositions.Length, freqList.Count);

        for (int i = 0; i < wireCount; i++)
        {
            Vector3 spawnPos = startPositions[i].position + new Vector3(0.5f, 0f, 0f);

            GameObject wire = Instantiate(
                wirePrefabs[Random.Range(0, wirePrefabs.Length)],
                spawnPos, Quaternion.identity, startPositions[i]);

            WireDrag wireDrag = wire.GetComponentInChildren<WireDrag>();
            int freq = freqList[i];   // já baralhado, sem repetição

            if (wireDrag != null)
            {
                wireDrag.frequency = freq;

                int matIndex = System.Array.IndexOf(frequencies, freq);
                Material mat = wireMaterials[matIndex];

                wireDrag.line.material = mat;
                wireDrag.GetComponentInChildren<Image>().material = mat;
            }
        }
    }

    public void OnPuzzleComplete()
    {
        animator.SetBool("canOpen", true);

        isActive = false; 
        wasActive = true;
        puzzledoor = true;

        game3DCamera.Priority = 10;
        Puzzlecam.Priority = 0;

        PlayerController.canplay = true;
        Cursor.lockState = CursorLockMode.Locked;

        PlayerController.moveSpeed = 10f;
        PlayerController.jumpForce = 10f;
    }
}

public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}
