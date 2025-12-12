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
    private Movement playerMovement;   // para comprobar si está escondido

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
        if (agent == null)
        {
            Debug.LogError("EnemyAI: Falta NavMeshAgent");
            enabled = false;
            return;
        }

        agent.updateRotation = false;
    }

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        //obtener referencia a Movement del jugador
        if (player != null)
            playerMovement = player.GetComponent<Movement>();

        patrolTimer = patrolWaitTime;
        patrolSpeed = agent.speed;
        SetRandomPatrolTarget();

        agent.stoppingDistance = Mathf.Max(agent.stoppingDistance, 0.5f);
        agent.angularSpeed = 120f;
    }

    void Update()
    {

        // si el jugador está escondido, el enemigo lo ignora por completo

        if (playerMovement != null && playerMovement.isHidden)
        {
            chasing = false;
            noiseTarget = null;
            timeSinceLastSeen = Mathf.Infinity;

            agent.speed = patrolSpeed;

            if (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance + 0.1f)
                SetRandomPatrolTarget();

            if (agent.hasPath)
                RotateTowards(agent.steeringTarget);

            return; // ignorar todo lo demás
        }

        // Prioriza seguir ruido
        if (noiseTarget != null)
        {
            FollowNoise();
            RotateTowards(agent.steeringTarget);
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

        if (agent.hasPath && agent.remainingDistance > agent.stoppingDistance)
            RotateTowards(agent.steeringTarget);
    }

    void ChasePlayer()
    {
        if (player == null) return;

        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }

    void Patrol()
    {
        agent.speed = patrolSpeed;

        if (!agent.hasPath || agent.pathPending)
        {
            return;
        }

        if (agent.remainingDistance <= agent.stoppingDistance + 0.1f)
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
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            patrolTarget = hit.position;
            agent.SetDestination(patrolTarget);
        }
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;

        // Si está escondido, nunca puede verlo

        if (playerMovement != null && playerMovement.isHidden)
            return false;

        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;
        Vector3 playerCenter = player.position + Vector3.up * 1f;
        Vector3 dir = playerCenter - eyePos;
        float distance = dir.magnitude;

        if (distance > visionDistance) return false;

        float angle = Vector3.Angle(transform.forward, dir);
        if (angle > visionAngle * 0.5f) return false;

        RaycastHit hit;
        if (Physics.Raycast(eyePos, dir.normalized, out hit, visionDistance, visionBlockingMask))
        {
            if (hit.transform == player || hit.transform.IsChildOf(player))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    void FollowNoise()
    {
        agent.speed = Random.Range(minNoiseSpeed, maxNoiseSpeed);
        if (noiseTarget == null)
        {
            noiseTarget = null;
            agent.ResetPath();
            return;
        }

        agent.SetDestination(noiseTarget.position);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            noiseTarget = null;
            agent.speed = patrolSpeed;
        }
    }

    public void HearNoise(Transform noiseObject, NoiseLevel level)
    {
        if (noiseObject == null) return;

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

    void RotateTowards(Vector3 worldTarget)
    {
        Vector3 dir = (worldTarget - transform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, manualTurnSpeed * Time.deltaTime);
    }

    void RotateTowards(Transform t)
    {
        if (t == null) return;
        RotateTowards(t.position);
    }
}
