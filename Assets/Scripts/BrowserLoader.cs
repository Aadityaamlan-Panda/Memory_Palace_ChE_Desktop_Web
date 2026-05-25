using UnityEngine;
using VoltstroStudios.UnityWebBrowser;
using VoltstroStudios.UnityWebBrowser.Core;

public class BrowserLoader : MonoBehaviour
{
    private BaseUwbClientManager manager;

    void Start()
    {
        manager = GetComponent<BaseUwbClientManager>();

        if (manager.browserClient != null && manager.browserClient.IsConnected)
        {
            LoadPage();
        }
        else
        {
            manager.browserClient.OnClientConnected += LoadPage;
        }
    }

    void LoadPage()
    {
        Debug.Log("Browser Connected!");
        manager.browserClient.LoadUrl("https://aadityaamlan-panda.github.io/SpacedRepetition/");
    }
}