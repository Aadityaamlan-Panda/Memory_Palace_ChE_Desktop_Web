using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class ConceptScrollController : MonoBehaviour
{
    [Header("UI")]
    public TextMeshPro contentText;

    [Header("API")]
    public APIManager apiManager;

    void Start()
    {
        if (apiManager == null)
            apiManager = FindObjectOfType<APIManager>();

        if (contentText == null)
        {
            Debug.LogError("❌ Content Text not assigned!");
            return;
        }

        StartCoroutine(LoadConcepts());
    }

    IEnumerator LoadConcepts()
    {
        yield return StartCoroutine(apiManager.GetAllConcepts(OnConceptsLoaded));
    }

    void OnConceptsLoaded(List<ConceptModel> concepts)
    {
        if (concepts == null || concepts.Count == 0)
        {
            contentText.text = "No concepts found.";
            return;
        }

        PopulateScroll(concepts);
    }

    void PopulateScroll(List<ConceptModel> concepts)
    {
        StringBuilder sb = new StringBuilder();

        foreach (var c in concepts)
        {
            sb.AppendLine($"<b>{c.title}</b>");
            sb.AppendLine($"Memory: {c.memoryObject}");

            // Handle default/null safely
            sb.AppendLine($"Strength: {c.strength}");
            sb.AppendLine($"Repetitions: {c.repetitions}");

            if (!string.IsNullOrEmpty(c.lastReviewed))
                sb.AppendLine($"Last Reviewed: {c.lastReviewed}");

            sb.AppendLine(""); // spacing
        }

        contentText.text = sb.ToString();

        Debug.Log($"✅ Scroll populated with {concepts.Count} items");
    }
}