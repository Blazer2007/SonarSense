using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    [SerializeField] private GameObject deathMenuUI;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private string currentSceneName;

    void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        deathMenuUI.SetActive(false);
    }

    public void ShowDeathMenu()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        deathMenuUI.SetActive(true);
    }

    public void OnRestartButton()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(currentSceneName);
    }

    public void OnMainMenuButton()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OnExitButton()
    {
        Application.Quit();
    }
}
