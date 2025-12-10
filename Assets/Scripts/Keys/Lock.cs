using UnityEngine;

public class Lock : MonoBehaviour
{
    [Header("Configuración del candado")]
    public string requiredKeyID;   // Qué llave abre este candado
    public bool isOpen = false;

    [Header("Opcional: animación o puerta")]
    public GameObject objectToUnlock;

    public void TryUnlock(PlayerInventory inv)
    {
        if (isOpen) return;

        if (inv.HasKey(requiredKeyID))
        {
            Unlock();
        }
        else
        {
            Debug.Log("No tienes la llave correcta (" + requiredKeyID + ")");
        }
    }

    private void Unlock()
    {
        isOpen = true;
        Debug.Log("Candado abierto: " + requiredKeyID);

        if (objectToUnlock != null)
            objectToUnlock.SetActive(false); // O reproducir animación
    }
}
