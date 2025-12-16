using UnityEngine;
using UnityEngine.UI;

public class LockInteract : MonoBehaviour
{
    [Header("Distancia de interacción")]
    public float interactDistance = 3f;

    [Header("Referencia al candado")]
    public Lock targetLock;

    [Header("UI de Progreso")]
    public Slider progressBar;
    public GameObject progressPanel;

    [Header("Tiempo para abrir")]
    public float totalTime = 5f;
    private float currentProgress = 0f;

    [Header("Ruido mientras se fuerza el candado")]
    public float noiseInterval = 0.5f;
    private float noiseTimer = 0f;

    [Header("Jugador")]
    public Transform player; // Ahora se asigna desde el Inspector

    private bool isInteracting = false;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player no asignado en LockInteract!");
        }

        if (progressBar != null)
        {
            progressBar.maxValue = totalTime;
            progressBar.value = 0f;
            progressBar.gameObject.SetActive(false);
        }

        if (progressPanel != null)
            progressPanel.SetActive(false);
    }

    private void Update()
    {
        if (player == null) return; // Seguridad

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactDistance && !targetLock.isOpen)
        {
            if (Input.GetKey(KeyCode.E))
            {
                StartInteraction();
            }
            else
            {
                StopInteraction();
            }
        }
        else
        {
            StopInteraction();
        }

        if (isInteracting)
        {
            currentProgress += Time.deltaTime;

            if (progressBar != null)
                progressBar.value = currentProgress;

            EmitNoiseWhileInteracting();

            if (currentProgress >= totalTime)
            {
                CompleteInteraction();
            }
        }
    }

    private void StartInteraction()
    {
        if (!isInteracting)
        {
            isInteracting = true;

            if (progressBar != null)
                progressBar.gameObject.SetActive(true);

            if (progressPanel != null)
                progressPanel.SetActive(true);
        }
    }

    private void StopInteraction()
    {
        if (isInteracting)
            isInteracting = false;

        if (progressBar != null)
            progressBar.gameObject.SetActive(false);

        if (progressPanel != null)
            progressPanel.SetActive(false);
    }

    private void CompleteInteraction()
    {
        PlayerInventory inv = player.GetComponent<PlayerInventory>();

        if (inv == null)
        {
            Debug.LogError("PlayerInventory no encontrado en el jugador.");
            return;
        }

        StopInteraction();
        currentProgress = 0f;

        if (progressBar != null)
            progressBar.value = 0f;

        if (progressPanel != null)
            progressPanel.SetActive(false);

        targetLock.TryUnlock(inv);
    }

    private void EmitNoiseWhileInteracting()
    {
        noiseTimer += Time.deltaTime;

        if (noiseTimer >= noiseInterval)
        {
            noiseTimer = 0f;

            EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();

            foreach (EnemyAI enemy in enemies)
            {
                enemy.HearNoise(transform, NoiseLevel.High);
            }
        }
    }
}
