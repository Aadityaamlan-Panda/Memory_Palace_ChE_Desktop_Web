using UnityEngine;

public class DoorIntX : MonoBehaviour
{
    public float openAngle = 90f;
    public float doorSpeed = 2f;

    [HideInInspector] public float interactDistance = 5f;

    private bool isOpen = false;


    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.localRotation;
        openRotation = Quaternion.Euler(openAngle, 0, 0) * closedRotation;
    }

    void Update()
    {
        // Smooth rotation
        if (isOpen)
            transform.localRotation = Quaternion.Lerp(transform.localRotation, openRotation, Time.deltaTime * doorSpeed);
        else
            transform.localRotation = Quaternion.Lerp(transform.localRotation, closedRotation, Time.deltaTime * doorSpeed);
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
    }
}