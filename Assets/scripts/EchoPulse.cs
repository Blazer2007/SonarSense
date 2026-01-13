using UnityEngine;

/*
 Esta script cria uma onda de som desde o ponto onde um objeto cai(se estiver dentro 
 do campo de audicao do jogador) e que vai ate ao jogador para mostrar o mapa.
 Sera utilizada pelo objeto EchoController para ter uma maneira global de detetar os 
 objetos e ativar as suas scripts respetivamente.
*/
public class EchoPulse : MonoBehaviour
{

    [Header("Player Hearing")]
    public Transform player; // Jogador
    public float hearingRange = 20f; // Alcance da audicao

    [Header("Pulse Settings")]
    public float pulseSpeed = 20f; // Velocidade da onda de som
    public float pulseThickness = 1.0f;

    private float currentDistance = 0f; // Distancia da onda de som
    private Vector3 pulseOrigin; // Origem da onda de som
    private bool pulseActive = false; // Verificacao da ativacao da onda de som

    void Start()
    {

    }

    public void StartPulse(Vector3 position) // Metodo que cria da onda de som apartir do ponto de colisao entre o objeto e o mapa
    {

        pulseOrigin = position; // Posicao de colisao dos objetos

        Shader.SetGlobalVector("_PulseOrigin", pulseOrigin); // Atribuicao da variavel anterior a propriedade "_PulseOrigin" do grafico de shaders
        Shader.SetGlobalFloat("_PulseDistance", 0f); // Atribuicao do valor 0 a distancia da onda de som(reset da distancia no grafico de shaders)
        Shader.SetGlobalFloat("_PulseTime", Time.time);
        Shader.SetGlobalFloat("_PulseThickness", pulseThickness);

        currentDistance = 0f; // Reset da distancia da onda de som
        pulseActive = true; // Afirmar que a onda de som ja foi criada
    }

    void Update()
    {
        if (!pulseActive) return;

        currentDistance += Time.deltaTime * pulseSpeed;
        Shader.SetGlobalFloat("_PulseDistance", currentDistance);

        float distToPlayer = Vector3.Distance(pulseOrigin, player.position);
        float maxDistance = Mathf.Max(hearingRange, distToPlayer);

        float stopBuffer = 20f;              // ja tens
        float fadeDistance = 10f;            // largura da zona de fade no fim

        float endDistance = maxDistance + stopBuffer;
        float fadeStart = endDistance - fadeDistance;

        // 1 ate comecar a zona de fade, depois desce ate 0
        float fade = 1f;
        if (currentDistance >= fadeStart)
        {
            float t = Mathf.InverseLerp(endDistance, fadeStart, currentDistance);
            fade = Mathf.Clamp01(t);
        }
        Shader.SetGlobalFloat("_PulseFade", fade);

        if (currentDistance >= endDistance)
        {
            pulseActive = false;
            // opcional: limpar completamente
            Shader.SetGlobalFloat("_PulseFade", 0f);
            Shader.SetGlobalFloat("_PulseDistance", 0f);
        }
    }

}
