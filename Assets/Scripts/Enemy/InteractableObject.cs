using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("Interacción")]
    public float interactionRadius = 3f;

    private bool isActive = false;
    private MonsterScript gm;

    private Transform player;
    private AudioSource audioSource;

    void Start()
    {
        gm = FindObjectOfType<MonsterScript>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Obtiene el AudioSource del objeto
        audioSource = GetComponent<AudioSource>();
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
