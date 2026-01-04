using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

enum EnemyState { Wander, InvestigateSound, ChasePlayer }

public class EnemyEcholocationAI : MonoBehaviour
{
    [Header("NavMesh")]
    public NavMeshAgent enemy;

    [Header("Wander Settings")]
    public float wanderRadius = 10f;      // raio máximo à volta do inimigo
    public float wanderInterval = 5f;     // tempo máximo entre novos destinos

    [Header("Hearing")]
    public float collisionHearingRadius = 20f;
    public float footstepHearingRadius = 8f;

    EnemyState state = EnemyState.Wander;
    Vector3 lastHeardPosition;
    Transform player;
    float wanderTimer;
    public float losePlayerDistance = 10f; // distância a partir da qual o inimigo desiste de perseguir o jogador
    public float chasepeed = 6f;


    void Start()
    {
        if (enemy == null)
            enemy = GetComponent<NavMeshAgent>();

        wanderTimer = 0f;
    }

    void Update()
    {
        float dt = Time.deltaTime;
        wanderTimer += dt;

        switch (state)
        {
            case EnemyState.Wander:
                enemy.speed = 3.5f; // Reduz a velocidade ao perder o jogador
                if (!enemy.pathPending &&
                    enemy.remainingDistance <= enemy.stoppingDistance &&
                    (!enemy.hasPath || enemy.velocity.sqrMagnitude < 0.01f))
                {
                    SetRandomDestination();
                    wanderTimer = 0f;
                }
                else if (wanderTimer >= wanderInterval)
                {
                    SetRandomDestination();
                    wanderTimer = 0f;
                }
                break;

            case EnemyState.InvestigateSound:
                enemy.speed = 3.5f; // Reduz a velocidade ao perder o jogador
                if (!enemy.pathPending && enemy.remainingDistance <= enemy.stoppingDistance)
                {
                    state = EnemyState.Wander;
                    wanderTimer = 0f;
                    SetRandomDestination();
                }
                break;

            case EnemyState.ChasePlayer:
                if (player != null)
                {
                    enemy.SetDestination(player.position);
                    enemy.speed = chasepeed; // Aumenta a velocidade ao perseguir o jogador

                    float distToPlayer = Vector3.Distance(transform.position, player.position);

                    // Se o jogador ficar demasiado longe, volta a andar aleatóriamente
                    if (distToPlayer > losePlayerDistance)
                    {
                        state = EnemyState.Wander;
                        player = null;
                        wanderTimer = 0f;
                        SetRandomDestination();
                    }
                    
                }
                else
                {
                    state = EnemyState.Wander;
                    wanderTimer = 0f;
                    SetRandomDestination();
                }
                break;
        }
    }

    void SetRandomDestination()
    {
        if (state != EnemyState.Wander) return;

        // ponto aleatório numa esfera em volta do inimigo
        Vector3 randomDir = Random.insideUnitSphere * wanderRadius;
        randomDir += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDir, out hit, wanderRadius, NavMesh.AllAreas))
        {
            enemy.SetDestination(hit.position);
        }
        //andar aleatóriamente e parar de andar depois de 3 segundos, andar mais rapido quando pressegue o jogador
    }

    public void HearCollision(Vector3 soundPos, float volume)
    {
        if (Vector3.Distance(transform.position, soundPos) > collisionHearingRadius)
            return;

        lastHeardPosition = soundPos;
        state = EnemyState.InvestigateSound;
        enemy.SetDestination(lastHeardPosition);
    }

    public void HearFootstep(Vector3 soundPos, Transform soundSource)
    {
        if (Vector3.Distance(transform.position, soundPos) > footstepHearingRadius)
            return;

            player = soundSource;
            state = EnemyState.ChasePlayer;
    }
}
