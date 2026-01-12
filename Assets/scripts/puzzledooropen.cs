using UnityEngine;

public class PuzzleDoorOpen : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PuzzleManager puzzleManager;

    void OnMouseDown()
    {
        if (puzzleManager.puzzledoor == true)
        {
            animator.SetBool("canOpen", true);
        }
        else
        {
            animator.SetBool("canOpen", false);
            Debug.Log("Door locked – puzzle not solved");
        }
    }
}