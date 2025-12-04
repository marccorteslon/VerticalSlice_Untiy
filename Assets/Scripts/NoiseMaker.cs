using UnityEngine;

public class NoiseMaker : MonoBehaviour
{
    public NoiseLevel noiseLevel = NoiseLevel.Low;

    [Header("Debug Settings")]
    public float noiseDuration = 0.2f;

    public void MakeNoise()
    {
        // Encontramos todos los enemigos
        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();

        foreach (EnemyAI enemy in enemies)
        {
            enemy.HearNoise(transform, noiseLevel);
        }

        Debug.Log($"{gameObject.name} hizo un ruido {noiseLevel}");
    }

    // Ejemplo de uso con colisiones o acción
    private void OnMouseDown()
    {
        MakeNoise(); // Hace ruido al clicar
    }
}
