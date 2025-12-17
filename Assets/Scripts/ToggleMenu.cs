using UnityEngine;

public class ToggleMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject mainPanel;              // Panel principal del menú
    public GameObject[] otherPanelsToClose;   // Otros paneles que se deben cerrar al cerrar el menú

    private bool isPaused = false;

    void Start()
    {
        // Aseguramos que todos los paneles estén ocultos al inicio
        if (mainPanel != null)
            mainPanel.SetActive(false);

        if (otherPanelsToClose != null)
        {
            foreach (var panel in otherPanelsToClose)
            {
                if (panel != null)
                    panel.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // Método público para botones
    public void TogglePause()
    {
        if (mainPanel == null) return;

        isPaused = !isPaused;
        mainPanel.SetActive(isPaused);

        if (isPaused)
        {
            // Pausar juego y mostrar cursor
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Reanudar juego y ocultar cursor
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Cerrar otros paneles si están abiertos
            if (otherPanelsToClose != null)
            {
                foreach (var panel in otherPanelsToClose)
                {
                    if (panel != null)
                        panel.SetActive(false);
                }
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
