using UnityEngine;
using System.Collections;

public class PlayerDeath : MonoBehaviour
{
    [Header("UI")]
    public GameObject deathPanel;
    public GameObject victoryPanel;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip deathSound;
    public AudioClip victorySound;

    [Header("Death Camera")]
    public Camera playerCamera;
    public float deathDelay = 1f;
    public float lookAtHeight = 1f;

    private bool isDead = false;
    private bool hasWon = false;

    void Start()
    {
        Time.timeScale = 1f;

        if (playerCamera == null)
            playerCamera = Camera.main;

        if (deathPanel != null)
            deathPanel.SetActive(false);

        if (victoryPanel != null)
            victoryPanel.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isDead && other.CompareTag("Enemy"))
        {
            Die(other.transform);
        }

        if (!hasWon && other.CompareTag("Victory"))
        {
            Win();
        }
    }

    void Die(Transform enemy)
    {
        isDead = true;

        StopEnemyMovementSounds(enemy);

        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        StartCoroutine(DeathSequence(enemy));
    }

    IEnumerator DeathSequence(Transform enemy)
    {
        float elapsed = 0f;

        while (elapsed < deathDelay)
        {
            LookAtEnemy(enemy);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        LookAtEnemy(enemy);

        if (deathPanel != null)
            deathPanel.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void LookAtEnemy(Transform enemy)
    {
        if (playerCamera == null || enemy == null) return;

        Vector3 targetPosition = enemy.position + Vector3.up * lookAtHeight;
        Vector3 direction = targetPosition - playerCamera.transform.position;

        if (direction.sqrMagnitude > 0.0001f)
            playerCamera.transform.rotation = Quaternion.LookRotation(direction.normalized);
    }

    void StopEnemyMovementSounds(Transform enemy)
    {
        if (enemy == null) return;

        EnemyAI enemyAI = enemy.GetComponentInParent<EnemyAI>();
        if (enemyAI == null)
            enemyAI = enemy.GetComponentInChildren<EnemyAI>();

        if (enemyAI != null)
        {
            enemyAI.StopMovementSounds();
            return;
        }

        // Fallback: si el collider que mata no tiene el EnemyAI en su jerarquia,
        // paramos igualmente los AudioSource del objeto que ha tocado al jugador.
        AudioSource[] sources = enemy.GetComponentsInChildren<AudioSource>(true);
        foreach (AudioSource source in sources)
        {
            source.Stop();
            source.clip = null;
        }
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
