using UnityEngine;
using System.Collections.Generic;

public class ConceptListUI : MonoBehaviour
{
    public APIManager api;

    private List<ConceptModel> allConcepts = new List<ConceptModel>();

    void Start()
    {
        Debug.Log("Starting API call...");
        StartCoroutine(api.GetAllConcepts(OnDataReceived));
    }

    void OnDataReceived(List<ConceptModel> concepts)
{
    if (concepts == null || concepts.Count == 0)
    {
        Debug.LogError("No concepts received!");
        return;
    }

    allConcepts = concepts;

    Debug.Log($"[TOTAL LOADED] {allConcepts.Count} concepts");

    SceneConceptLink[] sceneObjects = FindObjectsOfType<SceneConceptLink>();

    foreach (var obj in sceneObjects)
    {
        Debug.Log($"[PROCESSING OBJECT] {obj.memoryObjectKey}");

        obj.SetConcepts(allConcepts);
    }

    Debug.Log("All objects mapped using key matching!");
}
}