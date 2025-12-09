using UnityEngine;
using TMPro;

public class LockpickItem : MonoBehaviour
{
    public LockInteraction lockInteraction; // Referencia a la cerradura
    public TMP_Text keyStatusText;          // Referencia al texto de la UI

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (lockInteraction != null)
            {
                lockInteraction.canInteract = true;
                Debug.Log("Lockpick picked up");
            }

            // Cambiar el texto de la UI si está asignado
            if (keyStatusText != null)
            {
                keyStatusText.text = "Lockpick picked up";
            }

            Destroy(gameObject);
        }
    }
}
