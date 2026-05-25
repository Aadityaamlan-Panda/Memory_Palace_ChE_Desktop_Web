using UnityEngine;
using System.Collections.Generic;

public class ConceptInteractor : MonoBehaviour
{
    public AccurateSelect selector;

    // 🔹 OLD: Text attached to object
    public GameObject objectTextPrefab;

    // 🔹 NEW: Floating UI panel
    public GameObject uiPanelPrefab;

    private GameObject currentObject;

    // 🔹 Separate tracking
    private List<GameObject> spawnedObjectTexts = new List<GameObject>();
    private List<GameObject> spawnedUIPanels = new List<GameObject>();

    void Start()
    {
        if (selector == null)
        {
            Debug.LogError("AccurateSelect not assigned!");
            return;
        }

        selector.OnObjectSelected += HandleObjectSelected;
        selector.OnSelectionCleared += ClearAllUI;
        selector.OnKPressed += SpawnUIPanel; // 🔥 NEW
    }

    void OnDestroy()
    {
        if (selector != null)
        {
            selector.OnObjectSelected -= HandleObjectSelected;
            selector.OnSelectionCleared -= ClearAllUI;
            selector.OnKPressed -= SpawnUIPanel; // 🔥 NEW
        }
    }

    // =========================
    // HANDLE SELECTION
    // =========================
    void HandleObjectSelected(GameObject obj)
    {
        if (obj == null) return;

        /*if (currentObject == obj)
        {
            Debug.Log("[SKIP] Same object selected");
            return;
        }*/

        currentObject = obj;

        Debug.Log($"[INTERACTOR] Received: {obj.name}");

        ShowConceptsOnObject(obj);

        //SpawnUIPanel(); 
    }

    // =========================
    // SHOW TEXT ON OBJECT (OLD)
    // =========================
    void ShowConceptsOnObject(GameObject obj)
    {
        //ClearObjectTexts();

        SceneConceptLink link = obj.GetComponent<SceneConceptLink>();

        if (link == null)
        {
            Debug.LogWarning("No SceneConceptLink on object!");
            return;
        }

        List<ConceptModel> concepts = link.concepts;

        if (concepts == null || concepts.Count == 0)
        {
            Debug.Log($"[NO CONCEPTS] {obj.name}");
            return;
        }

        Debug.Log($"[SHOWING ON OBJECT] {obj.name} → {concepts.Count} concepts");

        foreach (ConceptModel concept in concepts)
        {

            Transform anchor = obj.transform.Find("TextAnchor");

            if (anchor == null)
            {
                Debug.LogWarning($"No TextAnchor found on {obj.name}");
                anchor = obj.transform;
            }

            GameObject item = Instantiate(objectTextPrefab, anchor);
            item.transform.localPosition = Vector3.zero;

            ConceptItem ui = item.GetComponent<ConceptItem>();
            if (ui != null)
            {
                ui.Setup(concept);
            }

            spawnedObjectTexts.Add(item);

            Debug.Log($"[SPAWNED OBJECT TEXT] {concept.title}");
        }
    }

    // =========================
    // SPAWN FLOATING UI (NEW)
    // =========================
   void SpawnUIPanel()
{
    ClearUIPanels();

    if (currentObject == null) return;

    // 🔥 MOVE TEXT UP FIRST
    

    SceneConceptLink link = currentObject.GetComponent<SceneConceptLink>();
    if (link == null) return;

    List<ConceptModel> concepts = link.concepts;
    if (concepts == null || concepts.Count == 0) return;

    Transform anchor = currentObject.transform.Find("TextAnchor");
    if (anchor == null) return;

    float offsetY = 0f; // 👈 BELOW the text now
    float sideOffset = 0f;

    foreach (ConceptModel concept in concepts)
    {
        Vector3 spawnPos =
            anchor.position /*+
            anchor.right * sideOffset +
            anchor.up * offsetY*/;

        GameObject panel = Instantiate(uiPanelPrefab, spawnPos, Quaternion.identity);

        panel.transform.rotation = anchor.rotation;
        ConceptImagePanel ui = panel.GetComponent<ConceptImagePanel>();
        if (ui != null)
        {
            ui.Setup(concept);
        }

        spawnedUIPanels.Add(panel);

        offsetY -= 0.3f;

        LiftTextAnchor();
    }
}
    // =========================
    // CLEAR FUNCTIONS
    // =========================
    void LiftTextAnchor()
{
    Transform anchor = currentObject.transform.Find("TextAnchor");

    if (anchor != null)
    {
        anchor.localPosition += new Vector3(0, 0.7f, 0); // adjust height
    }
}
    void ClearObjectTexts()
    {
        foreach (GameObject item in spawnedObjectTexts)
        {
            if (item != null)
                Destroy(item);
        }

        spawnedObjectTexts.Clear();
    }

    void ClearUIPanels()
    {
        foreach (GameObject panel in spawnedUIPanels)
        {
            if (panel != null)
                Destroy(panel);
        }

        spawnedUIPanels.Clear();
    }

    void ClearAllUI()
    {
        ClearObjectTexts();
        ClearUIPanels();

        currentObject = null;

        Debug.Log("[ALL UI CLEARED]");
    }
}