using UnityEngine;
using TMPro;
using System.Collections;

public class TutorialTextSequence : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text tutorialText;

    [Header("Tiempos")]
    public float initialDelay = 2f;
    public float showTime = 5f;
    public float waitTime = 2f;

    private string[] texts;

    void Start()
    {
        texts = new string[]
        {
            "The <color=#FFD700>emergency lights</color> on top of the doorframes will show you the direction wich the <color=#FF0000>enemy</color> is trying to enter.",

            "To close a <color=#FFD700>window</color>, get close to it and press <color=#FFD700>E</color>.",

            "To open <color=#00FF00>doors</color> you need a <color=#00FF00>key</color> the same color of its <color=#00FF00>lock</color>."
        };

        tutorialText.gameObject.SetActive(false);
        StartCoroutine(ShowSequence());
    }

    IEnumerator ShowSequence()
    {
        // Espera inicial antes del primer texto
        yield return new WaitForSeconds(initialDelay);

        for (int i = 0; i < texts.Length; i++)
        {
            tutorialText.text = texts[i];
            tutorialText.gameObject.SetActive(true);

            yield return new WaitForSeconds(showTime);

            tutorialText.gameObject.SetActive(false);

            // Espera entre textos (no después del último)
            if (i < texts.Length - 1)
                yield return new WaitForSeconds(waitTime);
        }
    }
}
