using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    // Nombre de la escena a cargar (debe estar en Build Settings)
    public string sceneName = "NombreDeLaEscena";

    public void LoadScene()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("LoadSceneButton: sceneName no está asignado.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }
}
