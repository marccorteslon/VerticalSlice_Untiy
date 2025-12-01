using UnityEngine;

public class ImpactHandler : MonoBehaviour
{
    [Header("Opcional: Mínima fuerza para considerar impacto")]
    public float impactThreshold = 0.1f;

    private void OnCollisionEnter(Collision collision)
    {
        // Velocidad relativa del impacto
        float impactForce = collision.relativeVelocity.magnitude;

        // Si supera el umbral, consideramos que ha habido impacto
        if (impactForce >= impactThreshold)
        {
            Debug.Log($"Impacto detectado con: {collision.gameObject.name} | Fuerza: {impactForce}");
        }
    }
}
