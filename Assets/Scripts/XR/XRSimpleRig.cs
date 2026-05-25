using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// FPS Rig — Memory House (desktop replacement for Meta XR rig).
///
/// WASD        = move
/// Mouse       = look (cursor auto-locked; Escape to release, RMB to re-lock)
/// F key       = door / slide / teleport  (was: left trigger)
/// G key       = concept text labels      (was: right trigger)
/// K key       = image panel              (was: right grip / secondary)
///
/// CROSSHAIR GAZE FEEDBACK  replaces the two controller ray colours:
///   Left  half GREEN  = left-type target in view  (door / slide / teleport)
///   Right half GREEN  = right-type target in view  (SceneConceptLink / concept)
///   Both  halves DIM  = nothing interactive in view
///
/// WIRING (Inspector):
///   Head Anchor     → your Camera transform (child of this GameObject)
///   Camera Offset   → GameObject that holds the Camera (for teleport correction)
///   Left Crosshair  → Image component, left  half of the on-screen crosshair
///   Right Crosshair → Image component, right half of the on-screen crosshair
///
/// SCENE SETUP — crosshair canvas:
///   1. Create a Canvas on the Camera (Screen Space Overlay, or World Space child of Camera).
///   2. Add an empty "Crosshair" child, centred (anchor + pivot = 0.5,0.5, pos = 0,0).
///   3. Inside Crosshair add two Image children — left half and right half.
///      Simple approach: use a thin horizontal bar sprite split down the middle,
///      or two small triangle / arrow sprites pointing away from centre.
///   4. Assign each Image to Left Crosshair / Right Crosshair in the Inspector.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class XRSimpleRig : MonoBehaviour
{
    [Header("Rig Anchors")]
    public Transform cameraOffset;
    public Transform headAnchor;        // assign your Camera transform here
    public Transform rightHandAnchor;   // kept for API compat — not used in FPS mode
    public Transform leftHandAnchor;    // kept for API compat — not used in FPS mode

    [Header("Crosshair Gaze Feedback")]
    [Tooltip("Left half of the crosshair — lights green when a door/slide/teleport is in view. " +
             "Mirrors what the left controller LineRenderer did in XR.")]
    public Image leftCrosshair;
    [Tooltip("Right half of the crosshair — lights green when a concept object is in view. " +
             "Mirrors what the right controller LineRenderer did in XR.")]
    public Image rightCrosshair;

    [Header("Crosshair Colours")]
    public Color crosshairActive  = new Color(0.1f, 1f, 0.3f, 1f);    // bright green
    public Color crosshairNeutral = new Color(1f,   1f, 1f,  0.35f);  // dim white

    [Header("Visuals (optional — scene-view debug ray)")]
    public LineRenderer rightRayLine;   // optional look-ray debug visual
    public LineRenderer leftRayLine;    // unused in FPS mode, kept for compat

    [Header("Movement")]
    public float moveSpeed  = 4f;
    public float flySpeed   = 3f;   // Q = up, E = down
    public float mouseSensX = 2f;
    public float mouseSensY = 2f;
    public float gravity    = -9.81f;

    [Header("Ray")]
    public float rayLength = 10f;

    // ── Private state ──────────────────────────────────────────────────────────
    CharacterController _cc;
    XRConceptManager    _mgr;

    float _pitch = 0f;
    float _yaw   = 0f;
    float _vv    = 0f;

    GameObject _lastHit;    // last concept object the ray touched (for K key)

    Gradient _gI, _gM;      // scene-view ray gradients (optional LineRenderer)

    // ── Lifecycle ─────────────────────────────────────────────────────────────
    void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }

    void Start()
    {
        _mgr = FindAnyObjectByType<XRConceptManager>();

        _yaw   = transform.eulerAngles.y;
        _pitch = headAnchor ? headAnchor.localEulerAngles.x : 0f;

        _gI = Grad(crosshairActive,  new Color(crosshairActive.r,  crosshairActive.g,  crosshairActive.b,  0f));
        _gM = Grad(crosshairNeutral, new Color(crosshairNeutral.r, crosshairNeutral.g, crosshairNeutral.b, 0f));

        var sh = Shader.Find("Hidden/Internal-Colored") ?? Shader.Find("Sprites/Default");
        if (rightRayLine) rightRayLine.material = new Material(sh);
        if (leftRayLine)  leftRayLine.material  = new Material(sh);

        SetCrosshair(leftCrosshair,  false);
        SetCrosshair(rightCrosshair, false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    static Gradient Grad(Color s, Color e)
    {
        var g = new Gradient();
        g.colorKeys = new GradientColorKey[] {
            new GradientColorKey(s, 0f),
            new GradientColorKey(e, 1f)
        };
        g.alphaKeys = new GradientAlphaKey[] {
            new GradientAlphaKey(s.a, 0f),
            new GradientAlphaKey(e.a, 1f)
        };
        return g;
    }

    // ── Update ─────────────────────────────────────────────────────────────────
    void Update()
    {
        HandleCursorToggle();
        HandleMouseLook();
        HandleMovement();
        HandleRayAndKeys();
    }

    // ── Cursor lock ────────────────────────────────────────────────────────────
    void HandleCursorToggle()
    {
        // Re-lock if cursor somehow escapes — click anywhere in game window
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible   = false;
            }
        }
    }

    // ── Mouse look ─────────────────────────────────────────────────────────────
    void HandleMouseLook()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        _yaw   += Input.GetAxis("Mouse X") * mouseSensX;
        _pitch -= Input.GetAxis("Mouse Y") * mouseSensY;
        _pitch  = Mathf.Clamp(_pitch, -80f, 80f);

        transform.rotation = Quaternion.Euler(0f, _yaw, 0f);

        if (headAnchor)
            headAnchor.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }

    // ── WASD + Q/E movement + gravity ──────────────────────────────────────────
    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = transform.right * h + transform.forward * v;
        move = Vector3.ClampMagnitude(move, 1f) * moveSpeed;

        // Q = fly up,  E = fly down
        // While either is held, override gravity so the player floats freely.
        bool flyUp   = Input.GetKey(KeyCode.Q);
        bool flyDown = Input.GetKey(KeyCode.E);

        if (flyUp || flyDown)
        {
            _vv     = 0f;   // cancel gravity while flying
            move.y  = (flyUp ? 1f : -1f) * flySpeed;
        }
        else
        {
            if (gravity == 0f)
            {
                _vv    = 0f;   // no gravity — hover in place
                move.y = 0f;
            }
            else
            {
                if (_cc.isGrounded) _vv = -1f;
                else                _vv += gravity * Time.deltaTime;
                move.y = _vv;
            }
        }

        _cc.Move(move * Time.deltaTime);
    }

    // ── Ray + crosshair feedback + F / G / K keys ──────────────────────────────
    void HandleRayAndKeys()
    {
        Transform cam = headAnchor ? headAnchor : transform;
        Vector3   org = cam.position;
        Vector3   dir = cam.forward;

        RaycastHit hit;
        bool anyHit   = Physics.Raycast(org, dir, out hit, rayLength,
                            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        bool leftHit  = anyHit && IsLeftTarget(hit.collider.gameObject);   // door/slide
        bool rightHit = anyHit && IsRightTarget(hit.collider.gameObject);  // concept

        // ── Crosshair colour — direct replacement for the two controller rays ──
        // In XR:  left  LineRenderer  turned _gI (green) on IsLeftTarget
        //         right LineRenderer  turned _gI (green) on IsRightTarget
        // Here:   left  Image         turns green        on IsLeftTarget
        //         right Image         turns green        on IsRightTarget
        SetCrosshair(leftCrosshair,  leftHit);
        SetCrosshair(rightCrosshair, rightHit);

        // Optional scene-view debug ray
        SetRay(rightRayLine, org, anyHit ? hit.point : org + dir * rayLength,
               leftHit || rightHit);

        if (rightHit) _lastHit = hit.collider.gameObject;

        // ── Key actions ─────────────────────────────────────────────────────────
        if (Input.GetKeyDown(KeyCode.F) && anyHit)
            FKey(hit.collider.gameObject, hit.point, org);

        // G key — shows both concept text labels AND image panel together
        if (Input.GetKeyDown(KeyCode.G))
        {
            var go = rightHit ? hit.collider.gameObject : _lastHit;
            if (go != null) { GKey(go); KKey(go); }
        }
    }

    // ── Crosshair helper ───────────────────────────────────────────────────────
    void SetCrosshair(Image img, bool active)
    {
        if (!img) return;
        img.color = active ? crosshairActive : crosshairNeutral;
    }

    // ── Optional scene-view ray ────────────────────────────────────────────────
    void SetRay(LineRenderer lr, Vector3 o, Vector3 e, bool interact)
    {
        if (!lr) return;
        lr.SetPosition(0, o);
        lr.SetPosition(1, e);
        lr.colorGradient = interact ? _gI : _gM;
    }

    // ── Interactability checks ─────────────────────────────────────────────────
    bool IsLeftTarget(GameObject go)
    {
        for (var t = go.transform; t != null; t = t.parent)
        {
            if (t.GetComponent<DoorInteract>()  != null) return true;
            if (t.GetComponent<DoorIntZ>()      != null) return true;
            if (t.GetComponent<DoorIntX>()      != null) return true;
            if (t.GetComponent<SlideX>()        != null) return true;
            if (t.GetComponent<SlideY>()        != null) return true;
            if (t.GetComponent<SlideZ>()        != null) return true;
            if (t.GetComponent<SlideCustom>()   != null) return true;
            if (t.gameObject.name == "Plane (1)" ||
                t.gameObject.name == "Plane (2)") return true;
        }
        if (go.GetComponentInParent<SlideZ>()      != null) return true;
        if (go.GetComponentInParent<SlideY>()      != null) return true;
        if (go.GetComponentInParent<SlideX>()      != null) return true;
        if (go.GetComponentInParent<SlideCustom>() != null) return true;
        return false;
    }

    bool IsRightTarget(GameObject go)
    {
        for (var t = go.transform; t != null; t = t.parent)
            if (t.GetComponent<SceneConceptLink>() != null) return true;
        return false;
    }

    // ── F key: door / slide / teleport ────────────────────────────────────────
    void FKey(GameObject go, Vector3 pt, Vector3 op)
    {
        for (var t = go.transform; t != null; t = t.parent)
        {
            var d  = t.GetComponent<DoorInteract>(); if (d)  { d.ToggleDoor();   return; }
            var dz = t.GetComponent<DoorIntZ>();     if (dz) { dz.ToggleDoor();  return; }
            var dx = t.GetComponent<DoorIntX>();     if (dx) { dx.ToggleDoor();  return; }
            var sx = t.GetComponent<SlideX>();       if (sx) { sx.ToggleSlide(); return; }
            var sy = t.GetComponent<SlideY>();       if (sy) { sy.ToggleSlide(); return; }
            var sz = t.GetComponent<SlideZ>();       if (sz) { sz.ToggleSlide(); return; }
            var sc = t.GetComponent<SlideCustom>();  if (sc) { sc.ToggleSlide(); return; }
        }
        var pSZ = go.GetComponentInParent<SlideZ>();      if (pSZ) { pSZ.ToggleSlide(); return; }
        var pSY = go.GetComponentInParent<SlideY>();      if (pSY) { pSY.ToggleSlide(); return; }
        var pSX = go.GetComponentInParent<SlideX>();      if (pSX) { pSX.ToggleSlide(); return; }
        var pSC = go.GetComponentInParent<SlideCustom>(); if (pSC) { pSC.ToggleSlide(); return; }

        if (go.name == "Plane (1)" || go.name == "Plane (2)")
            Teleport(new Vector3(pt.x, transform.position.y, pt.z));
    }

    // ── G key: concept text ────────────────────────────────────────────────────
    void GKey(GameObject go)
    {
        for (var t = go.transform; t != null; t = t.parent)
        {
            if (t.GetComponent<SceneConceptLink>() != null)
            {
                if (!_mgr) _mgr = FindAnyObjectByType<XRConceptManager>();
                _mgr?.OnConceptSelected("", t.gameObject);
                Debug.Log($"[XRSimpleRig] GKey: concept text on {t.gameObject.name}");
                return;
            }
        }
    }

    // ── K key: image panel ─────────────────────────────────────────────────────
    void KKey(GameObject go)
    {
        if (!_mgr) _mgr = FindAnyObjectByType<XRConceptManager>();
        if (!_mgr) return;
        for (var t = go.transform; t != null; t = t.parent)
        {
            if (t.GetComponent<SceneConceptLink>() != null)
            {
                _mgr.SpawnUIPanelForObject(t.gameObject);
                Debug.Log($"[XRSimpleRig] KKey: image panel on {t.gameObject.name}");
                return;
            }
        }
    }

    // ── Teleport ───────────────────────────────────────────────────────────────
    void Teleport(Vector3 dest)
    {
        _cc.enabled        = false;
        transform.position = dest;
        if (cameraOffset) cameraOffset.localPosition = Vector3.zero;
        _cc.enabled = true;
        _vv         = 0f;
    }
}