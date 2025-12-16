using UnityEngine;

public class UnpauseOnSceneStart : MonoBehaviour
{
    private void Awake()
    {
        // Si el juego está pausado, lo reanuda
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }
    }
}
