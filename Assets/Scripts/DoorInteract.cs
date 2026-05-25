using UnityEngine;

public class DoorInteract : MonoBehaviour
{
    public float openAngle = 90f;
    public float doorSpeed = 2f;

    // interactDistance kept as a field so existing Inspector values
    // don't cause missing-field errors, but it is no longer used here.
    // All ray detection is handled by XRSimpleRig.
    [HideInInspector] public float interactDistance = 5f;

    private bool isOpen = false;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.localRotation;
        openRotation   = Quaternion.Euler(0, openAngle, 0) * closedRotation;
    }

    void Update()
    {
        if (isOpen)
            transform.localRotation = Quaternion.Lerp(
                transform.localRotation, openRotation,   Time.deltaTime * doorSpeed);
        else
            transform.localRotation = Quaternion.Lerp(
                transform.localRotation, closedRotation, Time.deltaTime * doorSpeed);
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
    }
}