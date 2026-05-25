using UnityEngine;

/// <summary>
/// FPS door interaction — drop-in replacement for the Meta XR version.
///
/// Fixes vs original:
///   1. Collider is disabled while the door is swinging so the CharacterController
///      cannot fight the rotation and cause the door to snap back.
///   2. Uses Quaternion.RotateTowards instead of Lerp so the door fully arrives
///      at its target angle and the collider re-enables cleanly.
///   3. Opens AWAY from the opener (side-detection via dot product).
/// </summary>
public class XRDoorInteract : MonoBehaviour
{
    [Header("Door Settings")]
    public float openAngle  = 90f;
    public float doorSpeed  = 90f;  // degrees per second (was a Lerp factor before)

    [Header("Collider")]
    [Tooltip("The collider to disable while swinging. Auto-found on this GameObject if left empty.")]
    public Collider doorCollider;

    bool       _isOpen;
    Quaternion _closedRotation;
    Quaternion _openForward;
    Quaternion _openBackward;
    Quaternion _currentOpen;
    Quaternion _target;
    bool       _moving;
    bool       _initialised;

    void Awake() { Initialise(); }
    void Start()
    {
        Initialise();
        // Auto-find collider if not assigned
        if (!doorCollider) doorCollider = GetComponent<Collider>();
    }

    void Initialise()
    {
        if (_initialised) return;
        _closedRotation = transform.localRotation;
        _openForward    = Quaternion.Euler(0f,  Mathf.Abs(openAngle), 0f) * _closedRotation;
        _openBackward   = Quaternion.Euler(0f, -Mathf.Abs(openAngle), 0f) * _closedRotation;
        _target         = _closedRotation;
        _initialised    = true;
    }

    /// <summary>
    /// Opens away from openerWorldPos so the door never swings into the player.
    /// </summary>
    public void ToggleDoor(Vector3 openerWorldPos)
    {
        Initialise();
        _isOpen = !_isOpen;
        if (_isOpen)
        {
            float dot = Vector3.Dot(transform.forward,
                                    openerWorldPos - transform.position);
            _currentOpen = dot > 0f ? _openBackward : _openForward;
            _target      = _currentOpen;
        }
        else
        {
            _target = _closedRotation;
        }
        StartSwing();
    }

    /// <summary>
    /// Backward-compat overload — opens in the default forward direction.
    /// </summary>
    public void ToggleDoor()
    {
        Initialise();
        _isOpen = !_isOpen;
        _target = _isOpen ? _openForward : _closedRotation;
        if (_isOpen) _currentOpen = _openForward;
        StartSwing();
    }

    void StartSwing()
    {
        _moving = true;
        // Disable collider so CharacterController doesn't fight the rotating door
        if (doorCollider) doorCollider.enabled = false;
    }

    void Update()
    {
        if (!_initialised || !_moving) return;

        transform.localRotation = Quaternion.RotateTowards(
            transform.localRotation,
            _target,
            doorSpeed * Time.deltaTime);

        // Check if we've arrived (within 0.1 degrees)
        if (Quaternion.Angle(transform.localRotation, _target) < 0.1f)
        {
            transform.localRotation = _target;   // snap exactly to target
            _moving = false;
            // Re-enable collider now that the door has stopped moving
            if (doorCollider) doorCollider.enabled = true;
        }
    }
}