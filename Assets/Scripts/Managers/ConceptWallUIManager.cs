using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConceptWallUIManager : MonoBehaviour
{
    public APIManager api;

    public Transform contentParent;
    public GameObject conceptCardPrefab;

    public ManualVerticalLayout layout;

    void Start()
    {
        StartCoroutine(api.GetAllConcepts(OnDataLoaded));
    }

    void OnDataLoaded(List<ConceptModel> concepts)
    {
        foreach (var concept in concepts)
        {
            GameObject card = Instantiate(conceptCardPrefab, contentParent);

            var ui = card.GetComponent<ConceptCardUI>();
            ui.Setup(concept);
        }

        layout.Arrange(); 
    }
}