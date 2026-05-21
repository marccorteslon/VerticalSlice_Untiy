using UnityEngine;
using TMPro;

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

    [Header("Audio")]
    public AudioSource enterAudioSource;
    public AudioClip enterSound;

    [Header("UI")]
    public TextMeshProUGUI interactText;

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

        if (interactText != null)
        {
            interactText.text = "Press <color=yellow>E</color> to Close";
            interactText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning(name + ": interactText no está asignado en el Inspector.");
        }
    }

    public void Activate()
    {
        isActive = true;
        Debug.Log(name + " ACTIVADO");

        if (visualObject != null)
            visualObject.SetActive(true);

        if (animator != null)
            animator.SetTrigger("Abrir");
    }

    public void Deactivate()
    {
        isActive = false;
        Debug.Log(name + " DESACTIVADO");

        if (animator != null)
            animator.SetTrigger("Cerrar");

        if (visualObject != null)
            visualObject.SetActive(false);

        HideInteractText();
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
        if (player == null) return;

        if (!isActive)
        {
            HideInteractText();
            return;
        }

        float dist = Vector3.Distance(player.position, transform.position);
        bool canInteract = dist <= interactionRadius;

        Debug.Log(name + " | isActive: " + isActive + " | dist: " + dist + " | canInteract: " + canInteract);

        if (canInteract)
        {
            ShowInteractText();

            if (Input.GetKeyDown(KeyCode.E))
            {
                Deactivate();

                if (gm != null)
                    gm.NotifyObjectDeactivated();
            }
        }
        else
        {
            HideInteractText();
        }
    }

    void ShowInteractText()
    {
        if (interactText == null) return;

        interactText.text = "Press <color=yellow>E</color> to Close";
        interactText.gameObject.SetActive(true);
    }

    void HideInteractText()
    {
        if (interactText == null) return;

        interactText.gameObject.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }

    public void PlayMonsterEnterSound()
    {
        if (enterAudioSource != null && enterSound != null)
        {
            enterAudioSource.PlayOneShot(enterSound);
        }
    }
}