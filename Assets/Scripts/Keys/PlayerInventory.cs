using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    private List<KeyItem> keys = new List<KeyItem>();

    [Header("UI Llaves")]
    public List<KeyUI> keyUIs;

    private void Start()
    {
        // Asegurarse de que todas las imágenes empiezan ocultas
        foreach (var keyUI in keyUIs)
        {
            if (keyUI.image != null)
                keyUI.image.gameObject.SetActive(false);
        }
    }

    public void AddKey(KeyItem key)
    {
        if (keys.Contains(key))
            return;

        keys.Add(key);
        Debug.Log("Llave obtenida: " + key.keyID);

        // Activar imagen correspondiente
        foreach (var keyUI in keyUIs)
        {
            if (keyUI.key == key && keyUI.image != null)
            {
                keyUI.image.gameObject.SetActive(true);
                break;
            }
        }
    }

    public bool HasKey(string keyID)
    {
        foreach (var key in keys)
        {
            if (key.keyID == keyID)
                return true;
        }
        return false;
    }
}
