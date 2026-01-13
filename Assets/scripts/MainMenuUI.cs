using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game";
    public Slider mastVol, musicVol, sfxVol;
    public AudioMixer mainAudioMixer;
    public TMP_Dropdown graphicsDropdown; // ou Dropdown normal

    public void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Comecar no nivel atual
        Debug.Log("Start do MainMenuUI");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (graphicsDropdown != null)
        {
            graphicsDropdown.value = QualitySettings.GetQualityLevel();
            graphicsDropdown.RefreshShownValue();
        }
    }
    public void OnStartButton()
    {
        SceneTransition.Instance.TransitionToScene(gameSceneName);
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

    public void changeMasterVol()
    {
        mainAudioMixer.SetFloat("MasterVolum", mastVol.value);
    }

    public void changeMusicVol()
    {
        mainAudioMixer.SetFloat("MusicVolum", musicVol.value);
    }

    public void changeSFXVol()
    {
        mainAudioMixer.SetFloat("SFXVolum", sfxVol.value);
    }

    public void OnGraphicsChanged()
    {
        QualitySettings.SetQualityLevel(graphicsDropdown.value);
    }

}
