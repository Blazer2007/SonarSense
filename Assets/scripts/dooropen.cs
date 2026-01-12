using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    void OnMouseDown()
    {
        animator.SetBool("canOpen", true);
    }
}