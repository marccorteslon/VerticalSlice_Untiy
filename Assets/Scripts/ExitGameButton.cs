using UnityEngine;

public class ExitGameButton : MonoBehaviour
{
    // Llama a este método desde el OnClick() del botón en el UI
    public void ExitGame()
    {
#if UNITY_EDITOR
        // Funciona dentro del editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Funciona en build
        Application.Quit();
#endif
    }
}
