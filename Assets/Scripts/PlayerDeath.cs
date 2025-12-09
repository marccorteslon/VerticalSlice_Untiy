using UnityEngine;
using UnityEngine.UI;

public class PlayerDeath : MonoBehaviour
{
    [Header("UI")]
    public GameObject deathPanel;     // Panel que contiene el texto "Has muerto"

    private void Start()
    {
        // Oculta el panel al inicio
        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Si el objeto que colisiona tiene el tag Enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Die();
        }
    }

    void Die()
    {
        if (deathPanel != null)
            deathPanel.SetActive(true);

        Time.timeScale = 0f; // Congela el juego
    }
}
