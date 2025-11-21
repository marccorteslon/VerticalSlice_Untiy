using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;            // Referencia al jugador
    private NavMeshAgent agent;         // NavMeshAgent del enemigo


[Header("Chase Settings")]
    public float chaseDistance = 10f;   // Distancia máxima para perseguir

    [Header("Patrol Settings")]
    public float patrolRadius = 15f;    // Radio donde patrullará
    public float patrolWaitTime = 3f;   // Tiempo a esperar antes de moverse a otra posición

    private Vector3 patrolTarget;
    private float patrolTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        patrolTimer = patrolWaitTime;
        SetRandomPatrolTarget();
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= chaseDistance)
        {
            // Perseguir al jugador
            agent.SetDestination(player.position);
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            patrolTimer += Time.deltaTime;
            if (patrolTimer >= patrolWaitTime)
            {
                SetRandomPatrolTarget();
                patrolTimer = 0f;
            }
        }
    }

    void SetRandomPatrolTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            patrolTarget = hit.position;
            agent.SetDestination(patrolTarget);
        }
    }

}
