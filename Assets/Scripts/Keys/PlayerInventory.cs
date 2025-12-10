using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private List<KeyItem> keys = new List<KeyItem>();

    public void AddKey(KeyItem key)
    {
        keys.Add(key);
        Debug.Log("Llave obtenida: " + key.keyID);
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

