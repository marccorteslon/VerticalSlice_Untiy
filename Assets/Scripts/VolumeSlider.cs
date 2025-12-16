using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class VolumeSlider : MonoBehaviour
{
    [Header("Audio")]
    public AudioMixer audioMixer;              // Tu AudioMixer
    public string exposedParam = "MusicVol";   // Nombre del parámetro expuesto del grupo

    [Header("UI")]
    public Slider slider;
    public TextMeshProUGUI volumeText;

    [Header("Configuración")]
    [Range(0f, 1f)]
    public float defaultVolume = 0.75f;        // Valor por defecto si no hay guardado

    private string prefsKey; // Clave única para PlayerPrefs

    private void Start()
    {
        // Crear clave única para guardar la preferencia según el grupo
        prefsKey = exposedParam + "_Value";

        // Cargar valor guardado o asignar el valor por defecto definido en el Inspector
        float savedVolume = PlayerPrefs.GetFloat(prefsKey, defaultVolume);
        slider.value = savedVolume;
        UpdateVolume(savedVolume);

        // Escuchar cambios del slider
        slider.onValueChanged.AddListener(UpdateVolume);
    }

    private void UpdateVolume(float value)
    {
        // Convertir lineal (0-1) a dB
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(exposedParam, dB);

        // Guardar en PlayerPrefs
        PlayerPrefs.SetFloat(prefsKey, value);

        // Mostrar en porcentaje
        int percent = Mathf.RoundToInt(value * 100f);
        volumeText.text = percent + "%";
    }
}
