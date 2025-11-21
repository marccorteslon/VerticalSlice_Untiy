using UnityEngine;

public class SpawnLlave : MonoBehaviour
{
    [Header("Puntos de aparición")]
    public Transform[] spawnPoints;    // Asigna en el Inspector

    [Header("Objeto que aparecerá")]
    public GameObject targetObject;

    void Start()
    {
        SpawnInRandomPoint();
    }

    public void SpawnInRandomPoint()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No hay puntos asignados.");
            return;
        }

        // Elegir un punto aleatorio
        int index = Random.Range(0, spawnPoints.Length);

        // Mover el objeto al punto
        targetObject.transform.position = spawnPoints[index].position;

        // Activar si estaba desactivado
        if (!targetObject.activeSelf)
            targetObject.SetActive(true);
    }
}
