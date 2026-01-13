using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;    
        Cursor.visible = true;                     
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;                 
        isPaused = false;
    }

    public void OnMainMenuButton()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
        SceneTransition.Instance.TransitionToScene(mainMenuSceneName);
    }
    public void OnRestartButton()
    {
        Time.timeScale = 1f;
        SceneTransition.Instance.TransitionToScene("Game");
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void OnOptionsButton()
    {
        Debug.Log("Pause Options");
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = false;
    }

    public void OnExitButton()
    {
        Application.Quit();
    }
}
