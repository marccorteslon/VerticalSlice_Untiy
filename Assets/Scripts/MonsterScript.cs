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

    private InteractableObject currentActive;
    private bool waitingForPlayer = false;
    private float countdown = 0f;

    private bool monsterEntered = false;

    void Start()
    {
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        while (!monsterEntered)
        {
            // Reset UI
            monsterText.text = "";
            activeObjectText.text = "The monster is searching for an entrance...";
            timerText.text = "";

            // Espera aleatoria
            float waitTime = Random.Range(5f, 20f);
            yield return new WaitForSeconds(waitTime);

            // Elegir objeto aleatorio
            currentActive = objects[Random.Range(0, objects.Length)];
            currentActive.Activate();

            activeObjectText.text = "Entrance to close: " + currentActive.name;

            // Temporizador
            countdown = 10f;
            waitingForPlayer = true;

            while (waitingForPlayer && countdown > 0f)
            {
                countdown -= Time.deltaTime;
                timerText.text = "Time: " + countdown.ToString("F1");
                yield return null;
            }

            timerText.text = "";

            // Si sigue activo → el monstruo entra
            if (waitingForPlayer)
            {
                monsterEntered = true;
                currentActive.Deactivate();

                monsterText.text = "THE MONSTER HAS ENTERED!";
                activeObjectText.text = "";
                timerText.text = "";

                yield break;
            }
        }
    }

    public void NotifyObjectDeactivated()
    {
        waitingForPlayer = false;
        activeObjectText.text = "Entrance closed: " + currentActive.name;
    }
}
