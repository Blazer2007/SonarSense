using UnityEngine;

public class EchoObject : MonoBehaviour
{
    public float fadeDuration = 1.5f; // Duração das outlines dos objetos a desvanecerem

    private Renderer rend; // renderer do objeto
    private MaterialPropertyBlock block; 
    private float lastHitTime = -999f;

    void Start()
    {
        rend = GetComponent<Renderer>();
        block = new MaterialPropertyBlock();
    }

    void Update()
    {
        // Obter valores que o EchoPulse atualiza
        float pulseTime = Shader.GetGlobalFloat("_PulseTime");
        Vector3 pulseOrigin = Shader.GetGlobalVector("_PulseOrigin");
        float pulseDistance = Shader.GetGlobalFloat("_PulseDistance");
        float pulseThickness = Shader.GetGlobalFloat("_PulseThickness");

        // Distância ao centro do pulso
        float dist = Vector3.Distance(transform.position, pulseOrigin);

        // O anel tocou neste objeto?
        if (Mathf.Abs(dist - pulseDistance) <= pulseThickness)
        {
            lastHitTime = Time.time;
        }

        // Cálculo da visibilidade temporária
        float elapsed = Time.time - lastHitTime;
        float visibility = Mathf.Clamp01(1f - (elapsed / fadeDuration));

        // Aplicar ao material da instância
        rend.GetPropertyBlock(block);
        block.SetFloat("_EchoVisibility", visibility);
        rend.SetPropertyBlock(block);
    }
}
