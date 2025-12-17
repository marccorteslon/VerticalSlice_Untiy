using UnityEngine;

public class Footsteps : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;       // AudioSource donde se reproducirán los pasos
    public AudioClip stepClip;            // El clip de un solo paso
    public float pitchMin = 0.9f;         // Pitch mínimo
    public float pitchMax = 1.1f;         // Pitch máximo
    public float volume = 1f;             // Volumen del audio
    public float stepInterval = 0.2f;     // Tiempo entre pasos

    private float stepTimer = 0f;

    void Start()
    {
        if (audioSource == null)
        {
            Debug.LogWarning("No se asignó un AudioSource. Intentando obtener uno del mismo objeto.");
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource != null)
        {
            audioSource.clip = stepClip;
            audioSource.loop = false;  // No loop, control manual
            audioSource.playOnAwake = false;
            audioSource.volume = volume;
        }
        else
        {
            Debug.LogError("No se encontró ningún AudioSource. Asigna uno en el inspector.");
        }
    }

    void Update()
    {
        if (audioSource == null || stepClip == null)
            return;

        // Detecta si se pulsa alguna tecla WASD
        bool isMoving = Input.GetKey(KeyCode.W) ||
                        Input.GetKey(KeyCode.A) ||
                        Input.GetKey(KeyCode.S) ||
                        Input.GetKey(KeyCode.D);

        if (isMoving)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                audioSource.pitch = Random.Range(pitchMin, pitchMax);
                audioSource.PlayOneShot(stepClip, volume);
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f;  // Resetea el temporizador cuando no se mueve
        }
    }
}
