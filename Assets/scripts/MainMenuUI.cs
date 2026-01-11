using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game";

    public void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void OnStartButton()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnOptionsButton()
    {
        Debug.Log("Options");
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnExitButton()
    {
        Application.Quit();
    }
}
