using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [Header("UI")]
    public GameObject deathPanel;
    public GameObject victoryPanel;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip deathSound;
    public AudioClip victorySound;

    private bool isDead = false;
    private bool hasWon = false;

    void Start()
    {
        if (deathPanel != null)
            deathPanel.SetActive(false);

        if (victoryPanel != null)
            victoryPanel.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isDead && other.CompareTag("Enemy"))
        {
            Die();
        }

        if (!hasWon && other.CompareTag("Victory"))
        {
            Win();
        }
    }

    void Die()
    {
        isDead = true;

        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        if (deathPanel != null)
            deathPanel.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Win()
    {
        hasWon = true;

        if (audioSource != null && victorySound != null)
        {
            audioSource.PlayOneShot(victorySound);
        }

        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
