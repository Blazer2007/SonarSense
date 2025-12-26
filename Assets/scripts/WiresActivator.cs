using Unity.Cinemachine;
using UnityEngine;

public class WiresActivator : MonoBehaviour
{
    public GameObject WiresCanvas;
    public CinemachineCamera game3DCamera;
    public CinemachineCamera wires2DCamera;

    bool isActive = false;

    void OnMouseDown()
    {
        if (isActive) return;

        isActive = true;

        // Mostra minijogo
        WiresCanvas.SetActive(true);

        // Troca prioridades das câmaras
        game3DCamera.Priority = 0;
        wires2DCamera.Priority = 10;

        // Aqui também podes bloquear o controlo do player 3D
    }

    public void CloseMinigame()
    {
        isActive = false;

        // Esconde minijogo
        WiresCanvas.SetActive(false);

        // Volta à câmara 3D
        game3DCamera.Priority = 10;
        wires2DCamera.Priority = 0;

        // Reativa o controlo do player 3D
    }
}
