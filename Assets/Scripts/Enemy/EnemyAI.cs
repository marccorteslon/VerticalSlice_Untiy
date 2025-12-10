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

    [Header("Chase Settings")]
    public float chaseSpeed = 6f;
    public float loseSightTime = 3f;    // Tiempo que tarda en olvidarte
    private float timeSinceLastSeen = Mathf.Infinity;

    [Header("Vision Settings")]
    public float visionDistance = 15f;
    [Range(0, 360)]
    public float visionAngle = 90f;
    public float eyeHeight = 1.7f;      // altura del "ojo" respecto a transform.position
    public LayerMask visionBlockingMask = ~0; // capas que bloquean la visión (default: todo)

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
    public float manualTurnSpeed = 720f; // grados por segundo cuando miramos hacia objetivo

    private Transform noiseTarget;
    private bool chasing = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        // Ajustes por defecto recomendados
        if (agent == null)
        {
            Debug.LogError("EnemyAI: Falta NavMeshAgent");
            enabled = false;
            return;
        }

        // Permitir que NavMeshAgent maneje la rotación por defecto.
        agent.updateRotation = false; // controlar rotación manualmente para evitar error de rotacion
    }

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        patrolTimer = patrolWaitTime;
        patrolSpeed = agent.speed;
        SetRandomPatrolTarget();

        // Valores seguros por defecto
        agent.stoppingDistance = Mathf.Max(agent.stoppingDistance, 0.5f);
        agent.angularSpeed = 120f; // se usa por seguridad, pero controlamos rotación manualmente
    }

    void Update()
    {
        // Prioriza seguir ruido
        if (noiseTarget != null)
        {
            FollowNoise();
            RotateTowards(agent.steeringTarget); // girar suavemente hacia destino del agent
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

        // Si lo veo o lo he visto hace poco tiempo, persigo
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

        // Rotación manual para evitar patinaje: si el agent tiene destino, girar hacia su steering target
        if (agent.hasPath && agent.remainingDistance > agent.stoppingDistance)
        {
            RotateTowards(agent.steeringTarget);
        }
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

        // Si no tiene camino o ha llegado al objetivo, gestionar espera y elegir nuevo objetivo
        if (!agent.hasPath || agent.pathPending)
        {
            // esperar hasta que se calcule path
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

        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;
        Vector3 playerCenter = player.position + Vector3.up * 1f;
        Vector3 dir = playerCenter - eyePos;
        float distance = dir.magnitude;

        // 1. Comprobar distancia
        if (distance > visionDistance) return false;

        // 2. Comprobar ángulo de visión
        float angle = Vector3.Angle(transform.forward, dir);
        if (angle > visionAngle * 0.5f) return false;

        // 3. Raycast directo al jugador (considera solo capas que no bloquean la visión)
        RaycastHit hit;
        if (Physics.Raycast(eyePos, dir.normalized, out hit, visionDistance, visionBlockingMask))
        {
            // Si el primer hit no es el jugador, no lo ve
            if (hit.transform == player || hit.transform.IsChildOf(player))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Si no golpea nada (raro), consideramos que no ve
        return false;
    }

    void FollowNoise()
    {
        // velocidad variable al perseguir ruido
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
            // llegado al ruido, limpiar objetivo
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

    // Rotación manual suave hacia un objetivo (usa steeringTarget del NavMeshAgent)
    void RotateTowards(Vector3 worldTarget)
    {
        Vector3 dir = (worldTarget - transform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, manualTurnSpeed * Time.deltaTime);
    }

    // Overload para pasar transform o steering target
    void RotateTowards(Transform t)
    {
        if (t == null) return;
        RotateTowards(t.position);
    }
}
