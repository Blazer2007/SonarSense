using UnityEngine;
using UnityEngine.AI;

public class EnemyWander : MonoBehaviour
{
    public float wanderRadius = 10f;      // raio máximo à volta do inimigo
    public float wanderInterval = 3f;     // tempo entre escolhas de novo destino

    NavMeshAgent agent;
    float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderInterval;
        SetRandomDestination();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        // Quando chega ao destino OU passa o intervalo, escolhe novo ponto
        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance &&
            (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f))
        {
            SetRandomDestination();
            timer = wanderInterval;
        }
        else if (timer <= 0f)
        {
            SetRandomDestination();
            timer = wanderInterval;
        }
    }

    void SetRandomDestination()
    {
        // ponto aleatório numa esfera à volta do inimigo
        Vector3 randomDir = Random.insideUnitSphere * wanderRadius;
        randomDir += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDir, out hit, wanderRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}
