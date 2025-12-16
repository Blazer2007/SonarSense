using UnityEngine;

/*
 * Esta script cria uma onda de som desde o ponto onde um objeto cai(se estiver dentro do campo de audição do jogador) e que vai até ao jogador para mostrar o mapa.
 * Será utilizada pelo objeto EchoController para ter uma maneira global de detetar os objetos e ativar as suas scripts respetivamente.
*/
public class EchoPulse : MonoBehaviour
{

    [Header("Player Hearing")]
    public Transform player; // Jogador
    public float hearingRange = 20f; // Alcance da audição

    [Header("Pulse Settings")]
    public float pulseSpeed = 20f; // Velocidade da onda de som
    public float pulseThickness = 1.0f;

    private float currentDistance = 0f; // Distancia da onda de som
    private Vector3 pulseOrigin; // Origem da onda de som
    private bool pulseActive = false; // Verificação da ativação da onda de som

    void Start()
    {

    }

    public void StartPulse(Vector3 position) // Metodo que cria da onda de som apartir do ponto de colisão entre o objeto e o mapa
    {

        pulseOrigin = position; // Posição de colisão dos objetos

        Shader.SetGlobalVector("_PulseOrigin", pulseOrigin); // Atribuição da variavel anterior à propriedade "_PulseOrigin" do gráfico de shaders
        Shader.SetGlobalFloat("_PulseDistance", 0f); // Atribuição do valor 0 à distancia da onda de som(reset da distância no gráfico de shaders)
        Shader.SetGlobalFloat("_PulseTime", Time.time);
        Shader.SetGlobalFloat("_PulseThickness", pulseThickness);

        currentDistance = 0f; // Reset da distancia da onda de som
        pulseActive = true; // Afirmar que a onda de som já foi criada
    }

    void Update()
    {
        if (!pulseActive) return;

        currentDistance += Time.deltaTime * pulseSpeed;
        Shader.SetGlobalFloat("_PulseDistance", currentDistance);

        // Criação da variavel que guarda a distancia entre a origem do som e o jogador(para limitar a onda de som)
        float distToPlayer = Vector3.Distance(pulseOrigin, player.position);

        // Definir a distância MÁXIMA que a onda deve percorrer (o maior valor entre hearingRange e distToPlayer).
        float maxDistance = Mathf.Max(hearingRange, distToPlayer);

        // Adicionar uma margem de segurança (Buffer) à distância máxima.
        // Isto garante que a onda de som se afasta o suficiente de todos os objetos atingidos.
        float stopBuffer = 20f;

        if (currentDistance >= maxDistance + stopBuffer)
        {
            pulseActive = false; // A onda para apenas depois de passar a área de interesse
        }

        //PS. Pode ser preciso que, em vez de a onda chegar apenas ao jogador, passe por ele até uma certa distancia.Por exemplo no caso de haver inimigos que aparecam por de trás do jogador
    }
}
