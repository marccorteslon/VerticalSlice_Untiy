using UnityEngine;

public class TriggerAnimation : MonoBehaviour
{
    [Header("Animator a activar")]
    public Animator animator;

    [Header("Nombre del Trigger")]
    public string triggerName = "Activate";

    [Header("Audio")]
    public AudioSource audioSource; // AudioSource que reproducirá el sonido
    public AudioClip clip;          // Clip de sonido a reproducir
    public float delay = 0.5f;      // Retraso antes de reproducir el sonido

    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;

        if (other.CompareTag("Player"))
        {
            animator.SetTrigger(triggerName);
            activated = true;

            // Reproducir el sonido con retraso
            if (audioSource != null && clip != null)
                Invoke(nameof(PlaySound), delay);
        }
    }

    private void PlaySound()
    {
        audioSource.PlayOneShot(clip);
    }
}
