using UnityEngine;

public class LockpickItem : MonoBehaviour
{
    public LockInteraction lockInteraction; // Referencia a la cerradura

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (lockInteraction != null)
            {
                lockInteraction.canInteract = true;
                Debug.Log("Has recogido la ganzúa. Ya puedes interactuar.");
            }

            Destroy(gameObject);
        }
    }
}
