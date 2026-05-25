using UnityEngine;

/// <summary>
/// FPS concept interaction — drop-in replacement for the Meta XR version.
/// Oculus.Interaction dependency removed.
///
/// In the original XR setup this was wired via InteractableUnityEventWrapper.WhenSelect.
/// In the FPS build, XRSimpleRig fires GKey() which walks up the hierarchy and calls
/// XRConceptManager directly, so this component is no longer in the critical path.
///
/// It is kept so that:
///   a) Any existing Inspector references / UnityEvent wiring still compiles.
///   b) You can call OnSelected() manually from other scripts or UI buttons.
/// </summary>
public class XRConceptInteractable : MonoBehaviour
{
    [Tooltip("The concept ID — mirrors the one on the SceneConceptLink component")]
    public string conceptId = "";

    /// <summary>
    /// Can be called from a UnityEvent, a UI Button OnClick, or any other script.
    /// Notifies XRConceptManager that this concept has been selected.
    /// </summary>
    public void OnSelected()
    {
        Debug.Log($"[XRConceptInteractable] Selected: '{conceptId}' on {gameObject.name}");

        XRConceptManager mgr = FindAnyObjectByType<XRConceptManager>();
        if (mgr != null)
            mgr.OnConceptSelected(conceptId, gameObject);
    }
}