using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;

    [Header("Chase Settings")]
    public float chaseSpeed = 20f; // Velocidad al perseguir al jugador

    [Header("Vision Settings")]
    public float visionDistance = 15f; // Distancia máxima que puede ver
    [Range(0, 360)]
    public float visionAngle = 90f;    // Ángulo del campo de visión

    [Header("Patrol Settings")]
    public float patrolRadius = 15f;
    public float patrolWaitTime = 3f;
    private float patrolSpeed;

    private Vector3 patrolTarget;
    private float patrolTimer;

    [Header("Hearing Settings")]
    public float hearingRangeLow = 5f;
    public float hearingRangeMedium = 10f;
    public float hearingRangeHigh = 100f;
    public float minNoiseSpeed = 15f;
    public float maxNoiseSpeed = 25f;

    private Transform noiseTarget;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        patrolTimer = patrolWaitTime;
        patrolSpeed = agent.speed;
        SetRandomPatrolTarget();
    }

    void Update()
    {
        // Si hay ruido
        if (noiseTarget != null)
        {
            agent.speed = Random.Range(minNoiseSpeed, maxNoiseSpeed);
            agent.SetDestination(noiseTarget.position);
            Debug.Log($"Enemigo va hacia ruido en {noiseTarget.name} a velocidad {agent.speed}");

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                noiseTarget = null;
                agent.speed = patrolSpeed;
            }
            return;
        }

        // Comprobamos visión del jugador
        if (CanSeePlayer())
        {
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.speed = patrolSpeed;
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

    // Método que comprueba si el jugador está dentro del campo de visión
    bool CanSeePlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distance = directionToPlayer.magnitude;

        if (distance > visionDistance) return false; // Fuera del rango

        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > visionAngle / 2f) return false; // Fuera del ángulo

        // Opcional: raycast para evitar ver a través de paredes
        if (Physics.Raycast(transform.position + Vector3.up * 1.5f, directionToPlayer.normalized, out RaycastHit hit, visionDistance))
        {
            if (hit.transform != player) return false; // Obstáculo entre enemigo y jugador
        }

        return true;
    }

    public void HearNoise(Transform noiseObject, NoiseLevel level)
    {
        float distance = Vector3.Distance(transform.position, noiseObject.position);
        bool canHear = false;

        switch (level)
        {
            case NoiseLevel.Low:
                if (distance <= hearingRangeLow) canHear = true;
                break;
            case NoiseLevel.Medium:
                if (distance <= hearingRangeMedium) canHear = true;
                break;
            case NoiseLevel.High:
                if (distance <= hearingRangeHigh) canHear = true;
                break;
        }

        if (canHear)
        {
            noiseTarget = noiseObject;
            Debug.Log($"Enemigo escuchó ruido {level} de {noiseObject.name}");
        }
    }
}

public enum NoiseLevel
{
    Low,
    Medium,
    High
}
