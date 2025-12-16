using UnityEngine;

public class ToggleMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;

    private bool isPaused = false;

    void Start()
    {
        if (panel != null)
            panel.SetActive(false);
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
        if (panel == null) return;

        isPaused = !isPaused;
        panel.SetActive(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0f;                   // Pausa el juego
            Cursor.lockState = CursorLockMode.None; // Desbloquea el cursor
            Cursor.visible = true;                 // Hace visible el cursor
        }
        else
        {
            Time.timeScale = 1f;                   // Reanuda el juego
            Cursor.lockState = CursorLockMode.Locked; // Bloquea el cursor
            Cursor.visible = false;                // Oculta el cursor
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
