using UnityEngine;
using UnityEngine.AI;

public enum NoiseLevel
{
    Low,
    Medium,
    High
}

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    private NavMeshAgent agent;
    private Movement playerMovement;

    [Header("Chase Settings")]
    public float chaseSpeed = 6f;
    public float loseSightTime = 3f;
    private float timeSinceLastSeen = Mathf.Infinity;

    [Header("Vision Settings")]
    public float visionDistance = 15f;
    [Range(0, 360)]
    public float visionAngle = 90f;
    public float eyeHeight = 1.7f;
    public LayerMask visionBlockingMask = ~0;

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
    public float minNoiseSpeed = 3f;
    public float maxNoiseSpeed = 6f;

    [Header("Rotation Settings")]
    public float manualTurnSpeed = 720f;

    private Transform noiseTarget;
    private bool chasing = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
    }

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (player != null)
            playerMovement = player.GetComponent<Movement>();

        patrolSpeed = agent.speed;
        patrolTimer = patrolWaitTime;

        agent.stoppingDistance = Mathf.Max(agent.stoppingDistance, 0.5f);
        agent.angularSpeed = 120f;

        SetRandomPatrolTarget();
    }

    void Update()
    {
        // Jugador escondido = ignorar todo y patrullar
        if (playerMovement != null && playerMovement.isHidden)
        {
            ForcePatrol();
            RotateIfNeeded();
            return;
        }

        // Prioridad: ruido
        if (noiseTarget != null)
        {
            FollowNoise();
            RotateIfNeeded();
            return;
        }

        bool seesPlayer = CanSeePlayer();

        if (seesPlayer)
            timeSinceLastSeen = 0f;
        else
            timeSinceLastSeen += Time.deltaTime;

        if (timeSinceLastSeen < loseSightTime)
        {
            chasing = true;
            ChasePlayer();
        }
        else
        {
            chasing = false;
            Patrol();
        }

        RotateIfNeeded();
    }

    // BEHAVIOURS

    void ChasePlayer()
    {
        if (player == null)
        {
            ForcePatrol();
            return;
        }

        if (IsDestinationReachable(player.position))
        {
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
        }
        else
        {
            ForcePatrol();
        }
    }

    void Patrol()
    {
        agent.speed = patrolSpeed;

        if (!agent.hasPath)
        {
            SetRandomPatrolTarget();
            return;
        }

        if (agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            patrolTimer += Time.deltaTime;
            if (patrolTimer >= patrolWaitTime)
            {
                patrolTimer = 0f;
                SetRandomPatrolTarget();
            }
        }
    }

    void FollowNoise()
    {
        if (noiseTarget == null)
        {
            ForcePatrol();
            return;
        }

        if (IsDestinationReachable(noiseTarget.position))
        {
            agent.speed = Random.Range(minNoiseSpeed, maxNoiseSpeed);
            agent.SetDestination(noiseTarget.position);
        }
        else
        {
            ForcePatrol();
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            ForcePatrol();
        }
    }

    // PATROL TARGET

    void SetRandomPatrolTarget()
    {
        for (int i = 0; i < 15; i++)
        {
            Vector3 randomPos = Random.insideUnitSphere * patrolRadius + transform.position;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomPos, out hit, patrolRadius, NavMesh.AllAreas))
            {
                if (IsDestinationReachable(hit.position))
                {
                    patrolTarget = hit.position;
                    agent.SetDestination(patrolTarget);
                    return;
                }
            }
        }

        // Último recurso: quedarse con path válido
        agent.SetDestination(transform.position);
    }

    // SENSORS

    bool CanSeePlayer()
    {
        if (player == null) return false;
        if (playerMovement != null && playerMovement.isHidden) return false;

        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;
        Vector3 playerPos = player.position + Vector3.up;
        Vector3 dir = playerPos - eyePos;

        if (dir.magnitude > visionDistance) return false;
        if (Vector3.Angle(transform.forward, dir) > visionAngle * 0.5f) return false;

        if (Physics.Raycast(eyePos, dir.normalized, out RaycastHit hit, visionDistance, visionBlockingMask))
        {
            return hit.transform == player || hit.transform.IsChildOf(player);
        }

        return false;
    }

    public void HearNoise(Transform noiseObject, NoiseLevel level)
    {
        if (noiseObject == null) return;

        float dist = Vector3.Distance(transform.position, noiseObject.position);

        bool canHear =
            (level == NoiseLevel.Low && dist <= hearingRangeLow) ||
            (level == NoiseLevel.Medium && dist <= hearingRangeMedium) ||
            (level == NoiseLevel.High && dist <= hearingRangeHigh);

        if (canHear)
            noiseTarget = noiseObject;
    }

    // HELPERS

    bool IsDestinationReachable(Vector3 target)
    {
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(agent.transform.position, target, NavMesh.AllAreas, path))
        {
            return path.status == NavMeshPathStatus.PathComplete;
        }
        return false;
    }

    void ForcePatrol()
    {
        noiseTarget = null;
        chasing = false;
        timeSinceLastSeen = Mathf.Infinity;
        agent.speed = patrolSpeed;
        SetRandomPatrolTarget();
    }

    void RotateIfNeeded()
    {
        if (agent.hasPath && agent.remainingDistance > agent.stoppingDistance)
            RotateTowards(agent.steeringTarget);
    }

    void RotateTowards(Vector3 worldTarget)
    {
        Vector3 dir = worldTarget - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir.normalized);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            manualTurnSpeed * Time.deltaTime
        );
    }
}
