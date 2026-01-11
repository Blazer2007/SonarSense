using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenAnyKey : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}
