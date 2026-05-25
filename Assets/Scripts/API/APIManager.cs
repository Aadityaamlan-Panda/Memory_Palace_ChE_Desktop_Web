using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class APIManager : MonoBehaviour
{
    [SerializeField] private string baseUrl = "https://memory-palace-server-render.onrender.com/concepts";

    // GET ALL
    public IEnumerator GetAllConcepts(System.Action<List<ConceptModel>> callback)
    {
        int currentPage = 0;
        int totalPages = 1;

        // ✅ Use List instead of StringBuilder
        List<ConceptModel> allConcepts = new List<ConceptModel>();

        while (currentPage < totalPages)
        {
            string url = $"{baseUrl}?page={currentPage}&size=20";

            Debug.Log("Requesting: " + url);

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("API Failed → Switching to CSV fallback");
                    List<ConceptModel> fallbackData = CSVLoader.LoadConceptsFromCSV();
                    callback?.Invoke(fallbackData);
                    yield break;
                }

                string json = request.downloadHandler.text;

                Debug.Log("PAGE RECEIVED");

                // ✅ Parse page wrapper safely
                ConceptPage page = JsonUtility.FromJson<ConceptPage>(json);

                if (page == null || page.content == null)
                {
                    Debug.LogError("❌ JSON Parsing Failed → Using CSV");
                    List<ConceptModel> fallbackData = CSVLoader.LoadConceptsFromCSV();
                    callback?.Invoke(fallbackData);
                    yield break;
                }

                totalPages = page.totalPages;

                // ✅ Add concepts directly
                allConcepts.AddRange(page.content);

                currentPage++;
            }
        }

        Debug.Log($"ALL PAGES LOADED: {allConcepts.Count} concepts");

        // ✅ Correct type passed
        callback?.Invoke(allConcepts);
    }

    // POST
    public IEnumerator AddConcept(ConceptModel concept)
    {
        string json = JsonUtility.ToJson(concept);

        using (UnityWebRequest request = new UnityWebRequest(baseUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            Debug.Log(request.result == UnityWebRequest.Result.Success
                ? "Concept Added Successfully"
                : "POST Error: " + request.error);
        }
    }

    // PUT
    public IEnumerator UpdateConcept(long id, ConceptModel concept)
    {
        string json = JsonUtility.ToJson(concept);

        using (UnityWebRequest request = new UnityWebRequest($"{baseUrl}/{id}", "PUT"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                Debug.LogError("PUT Error: " + request.error);
        }
    }

    // DELETE
    public IEnumerator DeleteConcept(long id)
    {
        using (UnityWebRequest request = UnityWebRequest.Delete($"{baseUrl}/{id}"))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                Debug.LogError("DELETE Error: " + request.error);
        }
    }
}

[System.Serializable]
public class ConceptPage
{
    public ConceptModel[] content;
    public int totalPages;
}