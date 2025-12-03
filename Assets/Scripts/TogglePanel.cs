using UnityEngine;

public class TogglePanel : MonoBehaviour
{
    [Header("Panel a mostrar u ocultar")]
    public GameObject panel;

    public void Toggle()
    {
        if (panel == null)
        {
            Debug.LogWarning("TogglePanel: No hay panel asignado.");
            return;
        }

        bool isActive = panel.activeSelf;
        panel.SetActive(!isActive);
    }

    public void Open()
    {
        if (panel == null) return;
        panel.SetActive(true);
    }

    public void Close()
    {
        if (panel == null) return;
        panel.SetActive(false);
    }
}
