using UnityEngine;
using UnityEngine.UI;

public class LockInteraction : MonoBehaviour
{
    [Header("Configuración")]
    public bool canInteract = true;
    public float totalTime = 10f;

    [Header("UI")]
    public Slider progressBar;
    public GameObject progressPanel;

    private float currentProgress = 0f;
    private bool isPlayerNear = false;
    private bool isInteracting = false;

    // NUEVO: frecuencia de sonido
    public float noiseInterval = 0.5f;
    private float noiseTimer = 0f;

    void Start()
    {
        if (progressBar != null)
        {
            progressBar.maxValue = totalTime;
            progressBar.value = currentProgress;
            progressBar.gameObject.SetActive(false);
        }

        if (progressPanel != null)
            progressPanel.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNear && canInteract)
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

            EmitNoiseWhileLockpicking();   // NUEVO

            if (currentProgress >= totalTime)
            {
                currentProgress = totalTime;
                StopInteraction();

                if (progressBar != null)
                    progressBar.gameObject.SetActive(false);

                if (progressPanel != null)
                    progressPanel.SetActive(false);

                Debug.Log("Cerradura completada.");

                Destroy(gameObject);
            }
        }
    }

    private void EmitNoiseWhileLockpicking()
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
        {
            isInteracting = false;
        }

        if (!isPlayerNear)
        {
            if (progressBar != null)
                progressBar.gameObject.SetActive(false);

            if (progressPanel != null)
                progressPanel.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerNear = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            StopInteraction();
        }
    }
}
