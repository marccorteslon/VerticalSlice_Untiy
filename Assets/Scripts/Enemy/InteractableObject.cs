using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("Interacción")]
    public float interactionRadius = 3f;

    [Header("Grupo")]
    [Range(1, 4)]
    public int groupIndex = 1; // 1, 2, 3 o 4

    [Header("Visual")]
    public GameObject visualObject; // Nuevo objeto que se hará visible

    private bool isActive = false;
    private MonsterScript gm;

    private Transform player;
    private AudioSource audioSource;

    void Start()
    {
        gm = FindObjectOfType<MonsterScript>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        audioSource = GetComponent<AudioSource>();

        // Asegurarse de que el objeto visual esté inicialmente desactivado
        if (visualObject != null)
            visualObject.SetActive(false);
    }

    public void Activate()
    {
        isActive = true;
        Debug.Log(name + " ACTIVADO");

        if (visualObject != null)
            visualObject.SetActive(true); // Activamos el objeto visual
    }

    public void Deactivate()
    {
        isActive = false;
        Debug.Log(name + " DESACTIVADO");

        if (visualObject != null)
            visualObject.SetActive(false); // Desactivamos el objeto visual
    }

    public void PlayAudioLoop()
    {
        if (audioSource == null) return;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void StopAudio()
    {
        if (audioSource == null) return;
        audioSource.Stop();
    }

    void Update()
    {
        if (!isActive) return;

        float dist = Vector3.Distance(player.position, transform.position);

        if (dist <= interactionRadius && Input.GetKeyDown(KeyCode.E))
        {
            Deactivate();
            gm.NotifyObjectDeactivated();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
