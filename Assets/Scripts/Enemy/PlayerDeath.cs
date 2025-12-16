using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [Header("UI")]
    public GameObject deathPanel;
    public GameObject victoryPanel;    // Nueva pantalla de victoria

    [Header("Audio")]
    public AudioSource audioSource;    // AudioSource del jugador o global
    public AudioClip deathSound;       // Sonido de muerte
    public AudioClip victorySound;     // Sonido de victoria

    private bool isDead = false;
    private bool hasWon = false;

    void Start()
    {
        if (deathPanel != null)
            deathPanel.SetActive(false);

        if (victoryPanel != null)
            victoryPanel.SetActive(false);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!isDead && hit.gameObject.CompareTag("Enemy"))
        {
            Die();
        }

        if (!hasWon && hit.gameObject.CompareTag("Victory"))
        {
            Win();
        }
    }

    void Die()
    {
        isDead = true;

        // Reproduce el sonido de muerte
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        if (deathPanel != null)
            deathPanel.SetActive(true);

        // Pausa el juego
        Time.timeScale = 0f;

        // Libera el ratón para el menú de muerte
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Win()
    {
        hasWon = true;

        // Reproduce el sonido de victoria
        if (audioSource != null && victorySound != null)
        {
            audioSource.PlayOneShot(victorySound);
        }

        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        // Pausa el juego
        Time.timeScale = 0f;

        // Libera el ratón para el menú de victoria
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
