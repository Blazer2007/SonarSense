using UnityEngine;

public class PuzzleActivator : MonoBehaviour
{
    public PuzzleManager manager;
    void OnMouseDown()
    {
        // se o puzzle ja foi ativo, nao permite que se faca o puzzle novamente
        if (manager.wasActive) return;
        else // caso contrario, inicia o puzzle
        { 
            manager.StartPuzzle(); 
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
