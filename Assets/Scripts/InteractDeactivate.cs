using UnityEngine;

public class InteractDeactivate : MonoBehaviour
{
    [Header("Objeto a desactivar")]
    public GameObject objectToDeactivate;

    [Header("Configuración")]
    public KeyCode interactKey = KeyCode.E; // Tecla de interacción
    public float interactionDistance = 3f;   // Distancia máxima para interactuar

    private Transform player;

    void Start()
    {
        // Suponiendo que el jugador tiene el tag "Player"
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (objectToDeactivate == null)
        {
            Debug.LogWarning("No has asignado un objeto para desactivar.");
        }
    }

    void Update()
    {
        if (player == null || objectToDeactivate == null)
            return;

        // Calcula la distancia al jugador
        float distance = Vector3.Distance(player.position, transform.position);

        // Si el jugador está cerca y pulsa la tecla
        if (distance <= interactionDistance && Input.GetKeyDown(interactKey))
        {
            objectToDeactivate.SetActive(false);
            Debug.Log(objectToDeactivate.name + " ha sido desactivado.");
        }
    }

    // Opcional: dibuja la distancia de interacción en la escena
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
