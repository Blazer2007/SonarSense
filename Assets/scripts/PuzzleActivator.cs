using UnityEngine;

public class PuzzleActivator : MonoBehaviour
{
    public PuzzleManager manager;
    void OnMouseDown()
    {
        if (manager.isActive) return;

        manager.isActive = true;

        manager.StartPuzzle();
    }
}
