using UnityEngine;

[System.Serializable]
public class InteractableGroup
{
    [Header("Objetos decorativos del grupo")]
    public GameObject[] decorativeObjects;

    public void ActivateGroup()
    {
        foreach (GameObject obj in decorativeObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }
    }

    public void DeactivateGroup()
    {
        foreach (GameObject obj in decorativeObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }
}
