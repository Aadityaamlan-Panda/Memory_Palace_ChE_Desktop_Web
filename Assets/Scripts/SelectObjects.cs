using UnityEngine;

public class ObjectSelect : MonoBehaviour
{
    public float interactDistance = 5f;

    private Camera playerCamera;
    private GameObject selectedObject;

    void Start()
    {
        playerCamera = Camera.main;
    }

    void Update()
    {
        // Debug ray (same as your door script)
        Debug.DrawRay(playerCamera.transform.position,
                      playerCamera.transform.forward * interactDistance,
                      Color.green);

        if (Input.GetKeyDown(KeyCode.G))
        {
            RaycastHit hit;

            if (Physics.Raycast(playerCamera.transform.position,
                                playerCamera.transform.forward,
                                out hit,
                                interactDistance))
            {
                SelectObject(hit.collider.gameObject);
            }
            else
            {
                DeselectObject();
            }
        }
    }

    void SelectObject(GameObject obj)
    {
        // Deselect previous
        if (selectedObject != null)
        {
            SetHighlight(selectedObject, false);
        }

        selectedObject = obj;

        // Highlight new
        SetHighlight(selectedObject, true);

        Debug.Log("Selected: " + selectedObject.name);
    }

    void DeselectObject()
    {
        if (selectedObject != null)
        {
            SetHighlight(selectedObject, false);
            selectedObject = null;
        }
    }

    void SetHighlight(GameObject obj, bool state)
    {
        Renderer rend = obj.GetComponent<Renderer>();

        if (rend != null)
        {
            if (state)
                rend.material.color = Color.yellow; // selected
            else
                rend.material.color = Color.white;  // default
        }
    }
}