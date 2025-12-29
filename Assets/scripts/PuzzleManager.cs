using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    public CinemachineCamera Puzzlecam;
    public CinemachineCamera game3DCamera;
    public GameObject[] wirePrefabs;
    public Transform[] startPositions, slotPositions;
    public float[] frequencies;
    public int connectionsNeeded = 4;
    public int currentConnections = 0;
    public GameObject PuzzleCanvas;

    bool isActive = false;

    private void Start()
    {
        PuzzleCanvas = GameObject.Find("PuzzleCanvas");
    }
    void OnMouseDown()
    {
        if (isActive) return;

        isActive = true;

        // Mostrar minijogo
        PuzzleCanvas.SetActive(true);

        game3DCamera.Priority = 0;
        Puzzlecam.Priority = 10;
    }

    public void CloseMinigame()
    {
        isActive = false;

        PuzzleCanvas.SetActive(false);

        game3DCamera.Priority = 10;
        Puzzlecam.Priority = 0;
    }
    public void StartPuzzle()
    {
        Puzzlecam.gameObject.SetActive(true);

        // Gera fios pareados aleatoriamente
        List<float> freqList = new List<float>(frequencies);
        for (int i = 0; i < startPositions.Length; i++)
        {
            GameObject wire = Instantiate(wirePrefabs[Random.Range(0, wirePrefabs.Length)], startPositions[i]);
            float freq = freqList[Random.Range(0, freqList.Count)];
            wire.GetComponent<WireDrag>().frequency = freq;
            freqList.Remove(freq);
        }
    }

    public void OnPuzzleComplete()
    {
        Puzzlecam.gameObject.SetActive(false);
        // Recompensa ou volta ao 3D
    }
}
