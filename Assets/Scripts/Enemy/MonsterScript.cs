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

    private InteractableObject currentActive;
    private bool waitingForPlayer = false;
    private float countdown = 0f;
    public float timeToClose = 0f;

    private bool monsterEntered = false;

    void Start()
    {
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        while (!monsterEntered)
        {
            monsterText.text = "";
            activeObjectText.text = "The monster is searching for an entrance...";
            timerText.text = "";

            float waitTime = Random.Range(5f, 20f);
            yield return new WaitForSeconds(waitTime);

            currentActive = objects[Random.Range(0, objects.Length)];
            currentActive.Activate();

            activeObjectText.text = "Entrance to close: " + currentActive.name;

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

            timerText.text = "";

            // PARAR AUDIO SI SIGUE ACTIVO
            currentActive.StopAudio();

            if (waitingForPlayer)
            {
                // EL MONSTRUO ENTRA
                monsterEntered = true;
                currentActive.Deactivate();

                monsterText.text = "THE MONSTER HAS ENTERED!";
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

        // PARAR AUDIO SI CIERRAS LA ENTRADA
        currentActive.StopAudio();
    }

    IEnumerator ClearMonsterTextAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        monsterText.text = "";
    }

}
