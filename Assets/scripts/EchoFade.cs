using UnityEngine;

public class EchoFade : MonoBehaviour
{
    public float lifetime = 1.5f;      // tempo até desaparecer completamente
    private SpriteRenderer sr;
    private float timeElapsed = 0f;
    private Color startColor;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        startColor = sr.color;
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        float t = timeElapsed / lifetime;

        // Lerp do alpha de valor inicial até 0
        Color c = startColor;
        c.a = Mathf.Lerp(startColor.a, 0f, t);
        sr.color = c;

        if (timeElapsed >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
