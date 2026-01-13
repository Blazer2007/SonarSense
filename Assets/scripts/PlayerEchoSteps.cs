using UnityEngine;

public class PlayerEchoSteps : MonoBehaviour
{
    [Header("Prefab do passo")]
    public GameObject echoPrefab;

    [Header("Configuracao 3D")]
    public float distanceBetweenSteps = 0.5f;
    public float yOffset = -0.1f;  // ligeiramente debaixo do jogador
    public LayerMask groundLayer = 1;  // Layer do chao para raycast

    private Vector3 lastStepPosition;
    [SerializeField] private PlayerController playerController;

    void Start()
    {
        lastStepPosition = transform.position;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, lastStepPosition);
        if (distance >= distanceBetweenSteps)
        {
            SpawnStep3D();
            lastStepPosition = transform.position;
        }
    }

    void SpawnStep3D()
    {
        // Raycast para chao real (evita spawnar no ar)
        RaycastHit hit;
        Vector3 spawnPos = transform.position + Vector3.up * 1f; // Comeca um pouco acima

        if (Physics.Raycast(transform.position + Vector3.up * 1f, Vector3.down, out hit, 3f, groundLayer))
        {
            spawnPos = hit.point + Vector3.up * 0.01f; // Logo acima do chao
        }
        else
        {
            spawnPos = transform.position + Vector3.down * 0.1f; // Backup
        }
        if(playerController.isCrouching)
        {
            echoPrefab.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.13f); // Mais transparente ao agachar(faz menos barulho por isso nao faz tanto brilho)
        }
        else echoPrefab.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.3f); // Normal
            
        // Cria o "brilho" abaixo do jogador
        GameObject step = Instantiate(echoPrefab, spawnPos, Quaternion.Euler(90, 0, 0));
    }
}
