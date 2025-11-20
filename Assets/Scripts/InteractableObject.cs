using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("Interacción")]
    public float interactionRadius = 3f;   // Distancia para poder desactivarlo

    private bool isActive = false;
    private MonsterScript gm;

    private Transform player;  // referencia al jugador

    void Start()
    {
        gm = FindObjectOfType<MonsterScript>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Activate()
    {
        isActive = true;
        Debug.Log(name + " ACTIVADO");
    }

    public void Deactivate()
    {
        isActive = false;
        Debug.Log(name + " DESACTIVADO");
    }

    void Update()
    {
        if (!isActive) return;

        // Distancia al jugador
        float dist = Vector3.Distance(player.position, transform.position);

        // Si está dentro del radio → permitir desactivar
        if (dist <= interactionRadius)
        {
            Debug.Log("Cerca del objeto: " + name + " (puedes pulsar E)");

            if (Input.GetKeyDown(KeyCode.E))
            {
                Deactivate();
                gm.NotifyObjectDeactivated();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Dibuja el radio de interacción en la escena
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
