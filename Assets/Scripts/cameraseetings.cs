using UnityEngine;

public class cameraseetings : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Camera.main.fieldOfView = 70f;
        Camera.main.nearClipPlane = 0.1f;
        Camera.main.farClipPlane = 200f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
