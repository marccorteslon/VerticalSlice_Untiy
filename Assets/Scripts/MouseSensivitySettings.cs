using UnityEngine;
using UnityEngine.UI;

public class MouseSensitivitySettings : MonoBehaviour
{
    public Slider sensitivitySlider;

    [Header("Opcional")]
    public Movement playerMovement; // Se puede dejar vacío si no hay jugador

    public float minSensitivity = 200f;
    public float maxSensitivity = 1600f;
    public float defaultSensitivity = 800f;

    private const string PREF_KEY = "MouseSensitivity";

    void Start()
    {
        // Configurar slider
        sensitivitySlider.minValue = minSensitivity;
        sensitivitySlider.maxValue = maxSensitivity;

        // Cargar valor guardado
        float saved = PlayerPrefs.GetFloat(PREF_KEY, defaultSensitivity);
        sensitivitySlider.value = saved;

        // Aplicar al jugador si existe
        if (playerMovement != null)
            playerMovement.mouseSensitivity = saved;

        // Escuchar cambios del slider
        sensitivitySlider.onValueChanged.AddListener(OnSliderChanged);
    }

    void OnSliderChanged(float value)
    {
        // Guardar en PlayerPrefs
        PlayerPrefs.SetFloat(PREF_KEY, value);
        PlayerPrefs.Save();

        // Aplicar en tiempo real si hay jugador
        if (playerMovement != null)
            playerMovement.mouseSensitivity = value;
    }
}
