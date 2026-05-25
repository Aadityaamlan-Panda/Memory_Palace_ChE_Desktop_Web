using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class ConceptImagePanel : MonoBehaviour
{
    public long id;

    public string title;
    public string description;
    public string mediaUrl;

    public RawImage imageDisplay;

    // 🔥 Optional fallback texture (assign in inspector if you want)
    public Texture fallbackTexture;

    public void Setup(ConceptModel concept)
    {
        id = concept.id;

        title = concept.title;
        description = concept.description;
        mediaUrl = concept.mediaUrl;

        gameObject.name = title + "_Image";

        Debug.Log("Created Image Panel: " + title);

        if (imageDisplay == null)
        {
            Debug.LogError("❌ RawImage not assigned!");
            return;
        }

        if (string.IsNullOrEmpty(mediaUrl))
        {
            Debug.LogWarning("⚠️ Empty URL → using fallback");
            ApplyFallback();
            return;
        }

        // 🔥 Check if it's likely an image URL
        if (IsImageUrl(mediaUrl))
        {
            StartCoroutine(LoadImage(mediaUrl));
        }
        else
        {
            Debug.LogWarning("⚠️ Not an image URL → using fallback\nURL: " + mediaUrl);
            ApplyFallback();
        }
    }

    IEnumerator LoadImage(string url)
    {
        Debug.Log("Loading Image: " + url);

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ Image load failed: " + request.error);
            ApplyFallback();
        }
        else
        {
            Texture texture = DownloadHandlerTexture.GetContent(request);

            if (texture != null)
            {
                imageDisplay.texture = texture;
                Debug.Log("✅ Image Loaded");
            }
            else
            {
                Debug.LogWarning("⚠️ Texture null → fallback");
                ApplyFallback();
            }
        }
    }

    // 🔥 Simple check for image URL
    bool IsImageUrl(string url)
    {
        url = url.ToLower();

        return url.EndsWith(".png") ||
               url.EndsWith(".jpg") ||
               url.EndsWith(".jpeg") ||
               url.EndsWith(".webp");
    }

    // 🔥 Fallback handler
    void ApplyFallback()
    {
        if (fallbackTexture != null)
        {
            imageDisplay.texture = fallbackTexture;
        }
        else
        {
            // fallback color (visible signal)
            imageDisplay.color = Color.red;
        }

        Debug.Log("🟥 Fallback applied for: " + title);
    }
}