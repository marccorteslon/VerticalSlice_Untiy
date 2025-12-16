using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("Interacción")]
    public float interactionRadius = 3f;

    [Header("Grupo")]
    [Range(1, 4)]
    public int groupIndex = 1;

    [Header("Visual")]
    public GameObject visualObject;

    [Header("Animación")]
    public Animator animator;

    private bool isActive = false;
    private MonsterScript gm;
    private Transform player;
    private AudioSource audioSource;

    void Start()
    {
        gm = FindObjectOfType<MonsterScript>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        audioSource = GetComponent<AudioSource>();

        if (visualObject != null)
            visualObject.SetActive(false);
    }

    public void Activate()
    {
        isActive = true;
        Debug.Log(name + " ACTIVADO");

        if (visualObject != null)
            visualObject.SetActive(true);

        // TRIGGER: el monstruo intenta entrar
        if (animator != null)
            animator.SetTrigger("Abrir");
    }

    public void Deactivate()
    {
        isActive = false;
        Debug.Log(name + " DESACTIVADO");

        // TRIGGER: el jugador cierra la ventana
        if (animator != null)
            animator.SetTrigger("Cerrar");

        if (visualObject != null)
            visualObject.SetActive(false);
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
