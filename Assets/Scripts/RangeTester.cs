using UnityEngine;

public class RangeTester : MonoBehaviour
{
    public float detectionRadius = 5f; // El rango que quieres visualizar

    private void OnDrawGizmosSelected()
    {
        // Color del círculo (semi-transparente)
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        // Dibuja una esfera en la posición del objeto con el radio deseado
        Gizmos.DrawSphere(transform.position, detectionRadius);
    }
}
