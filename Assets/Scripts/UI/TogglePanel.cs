using UnityEngine;

public class TogglePanel : MonoBehaviour
{
    [Header("Panel principal")]
    public GameObject panel;

    [Header("Paneles a cerrar al abrir")]
    public GameObject[] panelsToClose;

    public void Toggle()
    {
        if (panel == null)
        {
            Debug.LogWarning("TogglePanel: No hay panel asignado.");
            return;
        }

        bool willOpen = !panel.activeSelf;

        if (willOpen)
            CloseOtherPanels();

        panel.SetActive(willOpen);
    }

    public void Open()
    {
        if (panel == null) return;

        CloseOtherPanels();
        panel.SetActive(true);
    }

    public void Close()
    {
        if (panel == null) return;
        panel.SetActive(false);
    }

    private void CloseOtherPanels()
    {
        if (panelsToClose == null) return;

        foreach (GameObject p in panelsToClose)
        {
            if (p != null && p != panel)
                p.SetActive(false);
        }
    }
}
