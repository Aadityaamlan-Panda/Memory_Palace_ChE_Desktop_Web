using UnityEngine;
using TMPro;

public class ConceptItem : MonoBehaviour
{
    public long id;

    public string title;
    public string description;
    public string mediaUrl;

    public string memoryObject;
    public string location;
    public string visualCue;

    public int strength;
    public int repetitions;

    public TextMeshPro titleText;
    public TextMeshPro descriptionText;

    // 🔥 NOW USING SPHERE RENDERER
    public Renderer strengthIndicator;

    public void Setup(ConceptModel concept)
    {
        id = concept.id;

        title = concept.title;
        description = concept.description;
        mediaUrl = concept.mediaUrl;

        memoryObject = concept.memoryObject;
        location = concept.location;
        visualCue = concept.visualCue;

        strength = concept.strength;
        repetitions = concept.repetitions;

        if (titleText != null)
            titleText.text = title;

        if (descriptionText != null)
            descriptionText.text = description;

        // 🔥 APPLY TRAFFIC LIGHT
        SetStrengthIndicator();

        gameObject.name = title;

        Debug.Log("Created Concept: " + title);
    }

    // =========================
    // 🔥 TRAFFIC LIGHT LOGIC (3D)
    // =========================
    void SetStrengthIndicator()
    {
        if (strengthIndicator == null) return;

        Color targetColor;

        if (strength > 3)
            targetColor = Color.green;
        else if (strength < 0)
            targetColor = Color.red;
        else
            targetColor = Color.yellow;

        // 🔥 IMPORTANT: Use material instance
        strengthIndicator.material.color = targetColor;
        strengthIndicator.material.EnableKeyword("_EMISSION");
        strengthIndicator.material.SetColor("_EmissionColor", targetColor * 3f);
    }
}