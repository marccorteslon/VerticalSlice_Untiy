using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class MonsterScript : MonoBehaviour
{
    [Header("Objetos del mapa")]
    public InteractableObject[] objects;

    [Header("UI Debug")]
    public TMP_Text activeObjectText;
    public TMP_Text timerText;
    public TMP_Text monsterText;

    [Header("Prefab a spawnear")]
    public GameObject spawnPrefab;

    [Header("Tiempo para cerrar entradas")]
    public float timeToClose = 5f;

    public float minWaitTime = 5f;
    public float maxWaitTime = 20f;

    [Header("Grupos de decorativos")]
    public InteractableGroup group1;
    public InteractableGroup group2;
    public InteractableGroup group3;
    public InteractableGroup group4;

    private InteractableObject currentActive;
    private bool waitingForPlayer = false;
    private float countdown = 0f;

    private bool monsterEntered = false;

    void Start()
    {
        // Inicialmente desactivamos todos los grupos
        group1?.DeactivateGroup();
        group2?.DeactivateGroup();
        group3?.DeactivateGroup();
        group4?.DeactivateGroup();

        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        while (!monsterEntered)
        {
            monsterText.text = "";
            activeObjectText.text = "The monster is searching for an entrance...";
            timerText.text = "";

            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            currentActive = objects[Random.Range(0, objects.Length)];
            currentActive.Activate();

            // ACTIVAR DECORATIVOS DEL GRUPO CORRESPONDIENTE
            ActivateGroupForObject(currentActive);

            //activeObjectText.text = "Entrance to close: " + currentActive.name;

            activeObjectText.text = "Close the window!";

            countdown = timeToClose;
            waitingForPlayer = true;

            // REPRODUCIR AUDIO EN BUCLE
            currentActive.PlayAudioLoop();

            while (waitingForPlayer && countdown > 0f)
            {
                countdown -= Time.deltaTime;
                timerText.text = "Time: " + countdown.ToString("F1");
                yield return null;
            }

            // DESACTIVAR DECORATIVOS DEL GRUPO
            DeactivateGroupForObject(currentActive);

            timerText.text = "";

            // PARAR AUDIO SI SIGUE ACTIVO
            currentActive.StopAudio();

            if (waitingForPlayer)
            {
                // EL MONSTRUO ENTRA
                monsterEntered = true;
                currentActive.Deactivate();

                monsterText.text = "YOU'RE NOT ALONE ANYMORE, BE CAREFUL.";
                StartCoroutine(ClearMonsterTextAfterDelay(4f)); // Limpiar el texto tras 2 segundos
                activeObjectText.text = "";
                timerText.text = "";

                if (spawnPrefab != null)
                {
                    Instantiate(spawnPrefab, currentActive.transform.position, Quaternion.identity);
                }
            }
        }
    }

    public void NotifyObjectDeactivated()
    {
        waitingForPlayer = false;
        activeObjectText.text = "Entrance closed: " + currentActive.name;

        // PARAR AUDIO SI EL JUGADOR CIERRA LA ENTRADA
        currentActive.StopAudio();
    }

    private void ActivateGroupForObject(InteractableObject obj)
    {
        switch (obj.groupIndex)
        {
            case 1:
                group1?.ActivateGroup();
                break;
            case 2:
                group2?.ActivateGroup();
                break;
            case 3:
                group3?.ActivateGroup();
                break;
            case 4:
                group4?.ActivateGroup();
                break;
        }
    }

    private void DeactivateGroupForObject(InteractableObject obj)
    {
        switch (obj.groupIndex)
        {
            case 1:
                group1?.DeactivateGroup();
                break;
            case 2:
                group2?.DeactivateGroup();
                break;
            case 3:
                group3?.DeactivateGroup();
                break;
            case 4:
                group4?.DeactivateGroup();
                break;
        }
    }

    // Corutina para limpiar el texto del monstruo después de un delay
    private IEnumerator ClearMonsterTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        monsterText.text = "";
    }
}
