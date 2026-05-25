using UnityEngine;
using System;
using System.Collections.Generic;
using VoltstroStudios.UnityWebBrowser.Core;
using UnityEngine.UI;

public class AccurateSelect : MonoBehaviour
{
    public float interactDistance = 5f;
    public LineRenderer lineRenderer;

    public KeyCode activateKey  = KeyCode.X;
    public KeyCode selectKey    = KeyCode.G;
    public KeyCode resetKey     = KeyCode.R;
    public KeyCode UIPanelKey   = KeyCode.K;

    // 🔥 BROWSER
    public BaseUwbClientManager browserManager;

    // 🔥 SCROLLER
    public ScrollRect scrollRect;

    // FirstPersonController removed — XRSimpleRig is the active player controller now.
    // The field is kept as a comment so other scripts that referenced it compile cleanly.
    // If any script calls accSelect.controller, replace that with your XRSimpleRig reference.

    private bool isBrowserMode = false;
    private bool isScrollMode  = false;

    private string currentUrl = "https://aadityaamlan-panda.github.io/SpacedRepetition/";

    // EVENTS
    public Action<GameObject> OnObjectSelected;
    public Action OnSelectionCleared;
    public Action OnKPressed;

    public GameObject CurrentSelectedObject { get; private set; }

    private Camera playerCamera;

    private GameObject currentHover;
    private GameObject selectedObject;

    private Vector3 smoothEndPoint;
    private bool isActive = false;

    private List<GameObject> selectionGroup = new List<GameObject>();
    private int currentIndex = -1;
    private GameObject currentRoot = null;

    void Start()
    {
        playerCamera = Camera.main;

        if (playerCamera == null)
        {
            Debug.LogError("Main Camera not found!");
            return;
        }

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.enabled = false;
        }

        smoothEndPoint = playerCamera.transform.position +
                         playerCamera.transform.forward * interactDistance;
    }

    void Update()
    {
        HandleModeToggle();

        if (isBrowserMode)
        {
            HandleBrowserControls();
            return;
        }

        if (isScrollMode)
        {
            HandleScrollControls();
            return;
        }

        if (Input.GetKeyDown(activateKey))
        {
            isActive = !isActive;
            if (lineRenderer != null)
                lineRenderer.enabled = isActive;
        }

        if (Input.GetKeyDown(UIPanelKey))
            OnKPressed?.Invoke();

        if (!isActive) return;

        if (Input.GetKeyDown(resetKey))
        {
            FullReset();
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (lineRenderer != null)
            lineRenderer.SetPosition(0, ray.origin);

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            HandleLine(hit.point);
            HandleHover(hit.collider.gameObject);

            if (Input.GetKeyDown(selectKey))
                SelectObject(hit.collider.gameObject);
        }
        else
        {
            HandleLine(ray.origin + ray.direction * interactDistance);
            ClearHover();
        }
    }

    // ── Mode toggle (Alt) ──────────────────────────────────────────────────────
    void HandleModeToggle()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            if (isBrowserMode || isScrollMode)
            {
                ExitAllModes();
                return;
            }

            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                if (hit.collider.CompareTag("BrowserTag"))
                    EnterBrowserMode();
                else if (hit.collider.CompareTag("ScrollTag"))
                    EnterScrollMode();
                else
                    Debug.LogWarning("❌ Not interactive surface");
            }
        }
    }

    void EnterBrowserMode()
    {
        isBrowserMode = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
        // Player movement is now on XRSimpleRig — cursor unlock stops mouse-look naturally.
        Debug.Log("🟢 ENTERED BROWSER MODE");
    }

    void EnterScrollMode()
    {
        isScrollMode = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
        Debug.Log("🟢 ENTERED SCROLL MODE");
    }

    void ExitAllModes()
    {
        isBrowserMode = false;
        isScrollMode  = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
        Debug.Log("🔴 EXITED ALL MODES");
    }

    // ── Browser controls ───────────────────────────────────────────────────────
    void HandleBrowserControls()
    {
        if (browserManager == null) return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Vector2 clickPos = new Vector2(Screen.width / 2, Screen.height / 2);
            browserManager.browserClient.SendMouseClick(clickPos, 1, 0, 0);
        }

        if (Input.GetKeyDown(KeyCode.Z)) browserManager.browserClient.GoBack();
        if (Input.GetKeyDown(KeyCode.C)) browserManager.browserClient.GoForward();
        if (Input.GetKeyDown(KeyCode.V)) browserManager.browserClient.LoadUrl(currentUrl);
    }

    // ── Scroll controls ────────────────────────────────────────────────────────
    void HandleScrollControls()
    {
        if (scrollRect == null) return;

        float scrollSpeed = 0.5f;

        float wheel = Input.GetAxis("Mouse ScrollWheel");
        scrollRect.verticalNormalizedPosition += wheel * scrollSpeed;

        if (Input.GetKey(KeyCode.UpArrow))
            scrollRect.verticalNormalizedPosition += scrollSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.DownArrow))
            scrollRect.verticalNormalizedPosition -= scrollSpeed * Time.deltaTime;
    }

    // ── Selection system ───────────────────────────────────────────────────────
    void HandleLine(Vector3 target)
    {
        smoothEndPoint = Vector3.Lerp(smoothEndPoint, target, Time.deltaTime * 8f);
        if (lineRenderer != null)
            lineRenderer.SetPosition(1, smoothEndPoint);
    }

    void HandleHover(GameObject obj)
    {
        SceneConceptLink link = obj.GetComponentInParent<SceneConceptLink>();
        GameObject root = link != null ? link.gameObject : obj;

        if (currentHover == root) return;
        ClearHover();
        currentHover = root;
        SetHighlight(currentHover, true, Color.cyan);
    }

    void ClearHover()
    {
        if (currentHover != null && currentHover != selectedObject)
        {
            SetHighlight(currentHover, false);
            currentHover = null;
        }
    }

    void SelectObject(GameObject obj)
    {
        SceneConceptLink link = obj.GetComponentInParent<SceneConceptLink>();
        if (link == null) return;

        GameObject root = link.gameObject;

        if (root != currentRoot)
        {
            BuildSelectionGroup(root);
            currentIndex = 0;
            currentRoot  = root;
        }
        else
        {
            currentIndex = (currentIndex + 1) % selectionGroup.Count;
        }

        GameObject selected = selectionGroup[currentIndex];

        foreach (var go in selectionGroup)
            SetHighlight(go, false);

        SetHighlight(selected, true, Color.yellow);

        selectedObject          = selected;
        CurrentSelectedObject   = selected;

        OnObjectSelected?.Invoke(root);
    }

    void BuildSelectionGroup(GameObject root)
    {
        selectionGroup.Clear();
        selectionGroup.Add(root);

        foreach (Transform t in root.GetComponentsInChildren<Transform>())
            if (t.gameObject != root)
                selectionGroup.Add(t.gameObject);
    }

    void FullReset()
    {
        foreach (var go in selectionGroup)
            SetHighlight(go, false);

        if (currentHover != null)
        {
            SetHighlight(currentHover, false);
            currentHover = null;
        }

        selectionGroup.Clear();
        currentIndex  = -1;
        currentRoot   = null;
        selectedObject          = null;
        CurrentSelectedObject   = null;

        OnSelectionCleared?.Invoke();
    }

    void SetHighlight(GameObject obj, bool state, Color color = default)
    {
        Renderer[] rends = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rends)
        {
            if (state)
            {
                r.material.EnableKeyword("_EMISSION");
                r.material.SetColor("_EmissionColor", color);
            }
            else
            {
                r.material.SetColor("_EmissionColor", Color.black);
                r.material.DisableKeyword("_EMISSION");
            }
        }
    }
}