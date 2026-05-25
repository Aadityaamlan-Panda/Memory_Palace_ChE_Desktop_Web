using UnityEngine;
using TMPro;

public class ConceptCardUI : MonoBehaviour
{
    public TextMeshPro titleText;
    public TextMeshPro memoryText;

    public void Setup(ConceptModel concept)
    {
        titleText.text = concept.title;
        memoryText.text = concept.memoryObject;
    }
}