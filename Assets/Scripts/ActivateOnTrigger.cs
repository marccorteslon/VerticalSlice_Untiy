using UnityEngine;

public class ActivateOnTrigger : MonoBehaviour
{
    [Header("Objeto a activar")]
    public GameObject objectToActivate;

    [Header("Opcional: tag del jugador")]
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }
        else
        {
            Debug.LogWarning("ActivateOnTrigger: No hay GameObject asignado.");
        }
    }
}
