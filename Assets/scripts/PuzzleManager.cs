using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LowLevel;
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
    public PlayerController PlayerController;
    public Canvas canvas;

    public bool isActive = false;

    private void Start()
    { 
    }
    void Update()
    {
            CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
            if (brain != null)
            {
                CinemachineCamera activeCam = brain.ActiveVirtualCamera as CinemachineCamera;
                if (activeCam != null)
                {
                    Debug.Log("Câmera ativa: " + activeCam.name);
                }
            }

    }
    public void ClosePuzzle()
    {
        isActive = false;

        game3DCamera.Priority = 10;
        Puzzlecam.Priority = 0;

        PlayerController.moveSpeed = 5f;
        PlayerController.jumpForce = 5f;

        Debug.Log("puzzle acabou");

    }
    public void StartPuzzle()
    {
        Debug.Log("puzzle comecou");

        PlayerController.moveSpeed = 0f;

        canvas.enabled = true;

        game3DCamera.enabled = false;

        game3DCamera.Priority = 0;
        Puzzlecam.Priority = 100;

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
