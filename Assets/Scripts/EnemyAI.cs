using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;

    [Header("Chase Settings")]
    public float chaseSpeed = 20f;
    public float loseSightTime = 3f;    // Tiempo que tarda en olvidarte
    private float timeSinceLastSeen = Mathf.Infinity;

    [Header("Vision Settings")]
    public float visionDistance = 15f;
    [Range(0, 360)]
    public float visionAngle = 90f;

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
        agent.angularSpeed = 720f; // ROTACIÓN MUCHO MÁS RÁPIDA

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
        // Si está siguiendo ruido, priorízalo
        if (noiseTarget != null)
        {
            agent.speed = Random.Range(minNoiseSpeed, maxNoiseSpeed);
            agent.SetDestination(noiseTarget.position);

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                noiseTarget = null;
                agent.speed = patrolSpeed;
            }
            return;
        }

        bool seesPlayer = CanSeePlayer();

        // Actualizar temporizador de memoria
        if (seesPlayer)
        {
            timeSinceLastSeen = 0f;
        }
        else
        {
            timeSinceLastSeen += Time.deltaTime;
        }

        // Si lo veo o lo he visto hace poco tiempo, sigo persiguiendo
        if (timeSinceLastSeen < loseSightTime)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void ChasePlayer()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }

    void Patrol()
    {
        agent.speed = patrolSpeed;

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

    bool CanSeePlayer()
    {
        if (player == null)
        {
            Debug.LogError("EnemyAI: Player es NULL. No tiene el tag 'Player' o no existe en escena.");
            return false;
        }

        Vector3 eyePos = transform.position + Vector3.up * 1.7f;
        Vector3 playerCenter = player.position + Vector3.up * 1f;
        Vector3 dir = playerCenter - eyePos;

        float distance = dir.magnitude;

        if (distance > visionDistance)
        {
            //Debug.Log("NO VE: fuera de distancia");
            return false;
        }

        float angle = Vector3.Angle(transform.forward, dir);
        if (angle > visionAngle / 2f)
        {
            //Debug.Log("NO VE: fuera del ángulo de visión");
            return false;
        }

        if (Physics.Raycast(eyePos, dir.normalized, out RaycastHit hit, visionDistance))
        {
            if (hit.transform != player)
            {
                Debug.Log("NO VE: el raycast golpea primero a " + hit.transform.name);
                return false;
            }
        }
        else
        {
            Debug.Log("NO VE: el Raycast NO golpea nada");
            return false;
        }

        Debug.Log("SI VE al jugador");
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
        }
    }
}

public enum NoiseLevel
{
    Low,
    Medium,
    High
}
