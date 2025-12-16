using UnityEngine;

public class TriggerAnimation : MonoBehaviour
{
    [Header("Animator a activar")]
    public Animator animator;

    [Header("Nombre del Trigger")]
    public string triggerName = "Activate";

    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;

        if (other.CompareTag("Player"))
        {
            animator.SetTrigger(triggerName);
            activated = true;
        }
    }
}
