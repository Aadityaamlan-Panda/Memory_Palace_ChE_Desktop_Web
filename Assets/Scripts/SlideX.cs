using UnityEngine;

public class SlideX : MonoBehaviour
{
    public float slideDistance = 2f;
    public float slideSpeed    = 2f;

    // interactDistance kept as a field so existing Inspector values
    // don't cause missing-field errors, but it is no longer used here.
    // All ray detection is handled by XRSimpleRig.
    [HideInInspector] public float interactDistance = 5f;

    private bool isOpen = false;

    private Vector3 closedPosition;
    private Vector3 openPosition;

    void Start()
    {
        closedPosition = transform.localPosition;
        openPosition   = closedPosition + new Vector3(slideDistance, 0, 0);
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