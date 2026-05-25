using UnityEngine;
using TMPro;
using System.Collections;

public class FlashcardManager : MonoBehaviour
{
    public GameObject flashcardPanel;
    public TMP_Text flashcardText;

    public float typingSpeed = 0.02f;

    private Coroutine typingCoroutine;

    void Start()
    {
        flashcardPanel.SetActive(false);
    }

    public void ShowFlashcard(GameObject obj)
    {
        flashcardPanel.SetActive(true);

        string message = "This is a flashcard for: " + obj.name +
                         "\n\nKeep pressing 'G' to cycle through children objects." +
                         "\nPoint somewhere else and press 'G' to deselect.";

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(message));
    }

    IEnumerator TypeText(string message)
    {
        flashcardText.text = "";

        foreach (char letter in message)
        {
            flashcardText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void HideFlashcard()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        flashcardPanel.SetActive(false);
    }
}