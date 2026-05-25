using UnityEngine;

public class AppManager : MonoBehaviour
{
    public APIManager api;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}