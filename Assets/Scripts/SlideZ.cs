using UnityEngine;

public class SlideZ : MonoBehaviour
{
    public float slideDistance = 2f;
    public float slideSpeed    = 2f;

    // Kept to avoid missing-field errors on existing scene objects.
    // No longer used — XRSimpleRig handles all ray detection.
    [HideInInspector] public float interactDistance = 5f;

    private bool isOpen = false;

    private Vector3 closedPosition;
    private Vector3 openPosition;

    void Start()
    {
        closedPosition = transform.localPosition;
        openPosition   = closedPosition + new Vector3(0, 0, slideDistance);
    }

    void Update()
    {
        if (isOpen)
            transform.localPosition = Vector3.Lerp(
                transform.localPosition, openPosition,   Time.deltaTime * slideSpeed);
        else
            transform.localPosition = Vector3.Lerp(
                transform.localPosition, closedPosition, Time.deltaTime * slideSpeed);
    }

    public void ToggleSlide()
    {
        isOpen = !isOpen;
    }
}