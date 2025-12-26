using Unity.Cinemachine;
using UnityEngine;

public class WiresManager : MonoBehaviour
{
    public CinemachineCamera game3DCamera;
    public CinemachineCamera wires2DCamera;
    public WireController[] wires;

    public void StartMinigame()
    {
        game3DCamera.Priority = 0;      // desativa 3D
        wires2DCamera.Priority = 10;    // ativa 2D
    }

    public void EndMinigame()
    {
        wires2DCamera.Priority = 0;
        game3DCamera.Priority = 10;
    }

    public void CheckAllWires()
    {
        bool allCorrect = true;

        foreach (var w in wires)
        {
            if (w.connectedPin == null)
            {
                allCorrect = false;
                break;
            }

            if (w.connectedPin.name != w.name.Replace("Wire", "EndPin"))
            {
                allCorrect = false;
                break;
            }
        }

        if (allCorrect)
        {
            Debug.Log("Puzzle concluído!");
            // Chamar lógica de completar tarefa (Script futura para abrir portas)
        }
    }
}
