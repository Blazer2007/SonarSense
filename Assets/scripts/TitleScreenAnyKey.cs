using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenAnyKey : MonoBehaviour
{

    void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneTransition.Instance.TransitionToScene("MainMenu");
        }
    }
}
