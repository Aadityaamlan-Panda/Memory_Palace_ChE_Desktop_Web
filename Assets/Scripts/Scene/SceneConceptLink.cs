using UnityEngine;
using System.Collections.Generic;

public class SceneConceptLink : MonoBehaviour
{
    public string memoryObjectKey;
    public Transform textAnchor;

    public List<ConceptModel> concepts = new List<ConceptModel>();

    public void SetConcepts(List<ConceptModel> allConcepts)
    {
        concepts.Clear();
        Debug.Log($"[PROCESSING OBJECT] {memoryObjectKey}");

        foreach (var concept in allConcepts)
        {
            if (concept.memoryObject == memoryObjectKey)
            {
                concepts.Add(concept);

                Debug.Log($"[MATCH FOUND] {memoryObjectKey} ← {concept.title}");
            }
        }

        Debug.Log($"[SUMMARY] {memoryObjectKey} has {concepts.Count} concepts");
    }

    public List<ConceptModel> GetConcepts()
    {
        return concepts;
    }
}

