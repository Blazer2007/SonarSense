using UnityEngine;

public class PuzzleActivator : MonoBehaviour
{
    public PuzzleManager manager;
    void OnMouseDown()
    {
        // se o puzzle já foi ativo, não permite que se faça o puzzle novamente
        if (manager.iswasActive) return;
        else // caso contrário, inicia o puzzle
            manager.StartPuzzle();
    }
}
