using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public KeyItem keyData; // La llave que representa este objeto

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inv = other.GetComponent<PlayerInventory>();

            if (inv != null)
            {
                inv.AddKey(keyData);
                Destroy(gameObject);
            }
        }
    }
}
