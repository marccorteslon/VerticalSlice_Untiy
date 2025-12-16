using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class InteractDeactivate : MonoBehaviour
{
    [Header("Objeto a desactivar")]
    public GameObject objectToDeactivate;

    [Header("Configuración")]
    public KeyCode interactKey = KeyCode.E;
    public float interactionDistance = 3f;

    [Header("Audio")]
    public AudioClip loopSound;

    private Transform player;
    private AudioSource audioSource;
    private bool hasInteracted = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = loopSound;
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        if (objectToDeactivate == null)
            Debug.LogWarning("No has asignado un objeto para desactivar.");

        if (loopSound == null)
            Debug.LogWarning("No has asignado un sonido en bucle.");
    }

    void Update()
    {
        if (hasInteracted || player == null || objectToDeactivate == null)
            return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactionDistance && Input.GetKeyDown(interactKey))
        {
            objectToDeactivate.SetActive(false);

            if (loopSound != null)
                audioSource.Play();

            hasInteracted = true;

            Debug.Log(objectToDeactivate.name + " ha sido desactivado y el sonido ha comenzado.");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
