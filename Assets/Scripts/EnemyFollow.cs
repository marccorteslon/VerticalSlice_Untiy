using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;           // Referencia al jugador
    private NavMeshAgent agent;        // NavMeshAgent del enemigo


public float chaseDistance = 10f;  // Distancia máxima para empezar a perseguir

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
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
            // Detenerse si está lejos
            agent.ResetPath();
        }
    }

}
