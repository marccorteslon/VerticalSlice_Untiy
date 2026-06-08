using UnityEngine;

public class ObjectGlow : MonoBehaviour
{
    [Header("Glow Settings")]
    public Color glowColor = Color.yellow;
    public float minIntensity = 1.5f;
    public float maxIntensity = 5f;
    public float pulseSpeed = 2f;

    private Renderer objectRenderer;
    private Material materialInstance;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();

        if (objectRenderer == null)
        {
            Debug.LogWarning("Este objeto no tiene Renderer.");
            return;
        }

        // Creamos una instancia del material para no modificar todos los objetos que usen el mismo material
        materialInstance = objectRenderer.material;

        // Activamos emission
        materialInstance.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        if (materialInstance == null) return;

        float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, t);

        Color finalColor = glowColor * intensity;
        materialInstance.SetColor("_EmissionColor", finalColor);
    }
}