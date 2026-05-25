using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// XR concept hub — handles concept text labels and UI image panels.
/// Text label spawns at TextAnchor and shifts UP.
/// UIPanel spawns at TextAnchor, BELOW the text (shifted down).
/// </summary>
public class XRConceptManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject objectTextPrefab;
    public GameObject uiPanelPrefab;

    [Header("Layout")]
    [Tooltip("How far UP the text label shifts from TextAnchor")]
    public float textUpOffset   = 0.25f;
    [Tooltip("How far DOWN the panel appears below the text")]
    public float panelDownOffset = 0.35f;

    [Header("Camera (auto-found)")]
    public Camera xrCamera;

    GameObject _currentObject;
    readonly List<GameObject> _texts  = new List<GameObject>();
    readonly List<GameObject> _panels = new List<GameObject>();

    void Start()
    {
        if (!xrCamera) xrCamera = Camera.main;
        AutoFind();
    }

    void AutoFind()
    {
        if (objectTextPrefab && uiPanelPrefab) return;
        var ci = FindAnyObjectByType<ConceptInteractor>();
        if (!ci) return;
        if (!objectTextPrefab) objectTextPrefab = ci.objectTextPrefab;
        if (!uiPanelPrefab)    uiPanelPrefab    = ci.uiPanelPrefab;
    }

    // ── Double left-click (G key): concept text labels ─────────
    public void OnConceptSelected(string conceptId, GameObject source)
    {
        if (!source) return;
        AutoFind();
        _currentObject = source;
        DestroyTexts();
        ShowTexts(source);
    }

    // ── Double right-click (K key): UI image panel ──────────────
    public void SpawnUIPanelForObject(GameObject source)
    {
        if (!source) return;
        AutoFind();
        _currentObject = source;

        var link = source.GetComponent<SceneConceptLink>();
        if (link == null || link.concepts == null || link.concepts.Count == 0)
        { Debug.Log("[XRConceptMgr] No concepts on " + source.name); return; }
        if (!uiPanelPrefab)
        { Debug.LogWarning("[XRConceptMgr] uiPanelPrefab null"); return; }

        DestroyPanels();
        //if (!xrCamera) xrCamera = Camera.main;

        // Find TextAnchor — panel spawns BELOW text
        Transform anchor = source.transform.Find("TextAnchor") ?? source.transform;

        foreach (var concept in link.concepts)
        {
            /*Vector3 spawnPos;
            Quaternion spawnRot;

            if (anchor != null)
            {
                // Panel below the text anchor
                spawnPos = anchor.position + Vector3.down * panelDownOffset;
                spawnRot = anchor.rotation;
            }
            else
            {
                // Fallback: 1.5m in front of camera
                spawnPos = xrCamera
                    ? xrCamera.transform.position + xrCamera.transform.forward * 1.5f
                    : source.transform.position + Vector3.up;
                spawnRot = xrCamera
                    ? Quaternion.LookRotation(xrCamera.transform.position - spawnPos)
                    : Quaternion.identity;
            }*/

            //var panel = Object.Instantiate(uiPanelPrefab, spawnPos, spawnRot);
            var panel = Object.Instantiate(uiPanelPrefab, anchor);
panel.transform.localPosition = Vector3.down * panelDownOffset;
panel.transform.localRotation = Quaternion.identity;

            // Assign worldCamera so WorldSpace canvas renders correctly
            var canvas = panel.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.worldCamera = xrCamera;
                canvas.sortingOrder = 100;
            }

            var cip   = panel.GetComponent<ConceptImagePanel>();
            if (cip != null) cip.Setup(concept);
            _panels.Add(panel);
            Debug.Log("[XRConceptMgr] Panel: " + concept.title + " for " + source.name);
        }
    }

    // ── Legacy grip fallback ────────────────────────────────────
    public void TriggerUIPanelForCurrent() => SpawnUIPanelForObject(_currentObject);

    // ── Helpers ─────────────────────────────────────────────────
    void ShowTexts(GameObject obj)
    {
        var link = obj.GetComponent<SceneConceptLink>();
        if (link == null || link.concepts == null || link.concepts.Count == 0)
        { Debug.Log("[XRConceptMgr] No concepts: " + obj.name); return; }
        if (!objectTextPrefab)
        { Debug.LogWarning("[XRConceptMgr] objectTextPrefab null"); return; }

        Transform anchor = obj.transform.Find("TextAnchor") ?? obj.transform;

        float yStack = textUpOffset;
        foreach (var concept in link.concepts)
        {
            var item = Object.Instantiate(objectTextPrefab, anchor);
            item.transform.localPosition = Vector3.up * yStack;
            var ui = item.GetComponent<ConceptItem>();
            if (ui) ui.Setup(concept);
            _texts.Add(item);
            yStack += 0.25f;
            Debug.Log("[XRConceptMgr] Text: " + concept.title + " on " + obj.name);
        }
    }

    void DestroyTexts()
    {
        foreach (var t in _texts) if (t) Object.Destroy(t);
        _texts.Clear();
    }

    void DestroyPanels()
    {
        foreach (var p in _panels) if (p) Object.Destroy(p);
        _panels.Clear();
    }

    public void ClearAll() { DestroyTexts(); DestroyPanels(); _currentObject = null; }
}
