using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Garante que o CanvasGroup existe
        if (fadeCanvasGroup == null)
            fadeCanvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        if (fadeCanvasGroup == null)
        {
            fadeCanvasGroup = GetComponent<CanvasGroup>();
        }
        StartCoroutine(FadeIn());
    }

    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    private IEnumerator FadeIn()
    {
        if (fadeCanvasGroup == null) yield break;

        fadeCanvasGroup.alpha = 0f; // Começa preto

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = 1f - (t / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 0f;
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {   
        if (fadeCanvasGroup == null)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
            yield break;
        }

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = t / fadeDuration;
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;

        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}
